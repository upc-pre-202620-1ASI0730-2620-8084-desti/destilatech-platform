using System.Net.Mime;
using DestilatechPlatform.API.OrdersReplenishment.Application.Services;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.ValueObjects;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Transform;
using DestilatechPlatform.API.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest;

/// <summary>
/// Customer orders (US18, US19). Registering an order discounts its products from stock,
/// and cancelling it returns them.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Orders")]
public class OrdersController(
    IOrderCommandService orderCommandService,
    IOrderQueryService orderQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("List the orders of an account")]
    [EndpointDescription("Returns the order history of the account, newest first. Use customerId to filter by customer.")]
    [ProducesResponseType(typeof(IEnumerable<OrderResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetOrders([FromQuery] int userId, [FromQuery] int? customerId,
        CancellationToken cancellationToken)
    {
        if (userId <= 0)
        {
            return UserIdRequired();
        }

        var orders = customerId is > 0
            ? await orderQueryService.Handle(new GetOrdersByCustomerIdQuery(customerId.Value, userId), cancellationToken)
            : await orderQueryService.Handle(new GetAllOrdersByUserIdQuery(userId), cancellationToken);
        return Ok(orders.Select(OrderResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Get an order")]
    [ProducesResponseType(typeof(OrderResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetOrderById(int id, [FromQuery] int userId, CancellationToken cancellationToken)
    {
        var order = await orderQueryService.Handle(new GetOrderByIdQuery(id, userId), cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        return Ok(OrderResourceFromEntityAssembler.ToResourceFromEntity(order));
    }

    [HttpPost]
    [EndpointSummary("Register an order")]
    [EndpointDescription("Registers an order in PENDING status and discounts its products from stock. "
                         + "Product names and prices are taken from the inventory, and the code is generated (#1001, #1002...).")]
    [ProducesResponseType(typeof(OrderResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RegisterOrder([FromBody] CreateOrderResource resource,
        [FromQuery] int userId = 0, CancellationToken cancellationToken = default)
    {
        var accountId = userId > 0 ? userId : resource.UserId ?? 0;
        if (accountId <= 0)
        {
            return UserIdRequired();
        }

        var command = RegisterOrderCommandFromResourceAssembler.ToCommandFromResource(resource, accountId);
        var result = await orderCommandService.Handle(command, cancellationToken);
        return result.Match<ActionResult>(
            order => CreatedAtAction(nameof(GetOrderById), new { id = order.Id, userId = accountId },
                OrderResourceFromEntityAssembler.ToResourceFromEntity(order)),
            error => ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(error, this));
    }

    [HttpPatch("{id:int}")]
    [EndpointSummary("Change the status of an order")]
    [EndpointDescription("Moves the order to its next status (PENDING, PREPARING, SHIPPED, DELIVERED) "
                         + "or cancels it with CANCELLED, which returns its products to stock.")]
    [ProducesResponseType(typeof(OrderResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusResource resource,
        [FromQuery] int userId, CancellationToken cancellationToken)
    {
        if (userId <= 0)
        {
            return UserIdRequired();
        }

        OrderStatus requestedStatus;
        try
        {
            requestedStatus = OrdersReplenishmentValueAssembler.ToEnumFromValue<OrderStatus>(resource.Status);
        }
        catch (ArgumentException exception)
        {
            return ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(
                OrdersReplenishmentError.InvalidData(exception.Message), this);
        }

        if (requestedStatus == OrderStatus.Cancelled)
        {
            var cancelResult = await orderCommandService.Handle(new CancelOrderCommand(id, userId), cancellationToken);
            return ToActionResult(cancelResult);
        }

        var order = await orderQueryService.Handle(new GetOrderByIdQuery(id, userId), cancellationToken);
        if (order is null)
        {
            return ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(
                OrdersReplenishmentError.OrderNotFound(id), this);
        }

        if (order.Status == requestedStatus)
        {
            return Ok(OrderResourceFromEntityAssembler.ToResourceFromEntity(order));
        }

        if (order.NextStatus() != requestedStatus)
        {
            var error = order.IsOpen()
                ? OrdersReplenishmentError.InvalidStatusTransition(
                    OrdersReplenishmentValueAssembler.ToValueFromEnum(order.Status),
                    OrdersReplenishmentValueAssembler.ToValueFromEnum(requestedStatus))
                : OrdersReplenishmentError.OrderClosed();
            return ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(error, this);
        }

        var advanceResult = await orderCommandService.Handle(new AdvanceOrderStatusCommand(id, userId),
            cancellationToken);
        return ToActionResult(advanceResult);
    }

    private ActionResult ToActionResult(Result<Order> result)
    {
        return result.Match<ActionResult>(
            order => Ok(OrderResourceFromEntityAssembler.ToResourceFromEntity(order)),
            error => ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(error, this));
    }

    private ActionResult UserIdRequired()
    {
        return ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(
            OrdersReplenishmentError.InvalidData("The userId of the account is required."), this);
    }
}
