using System.Net.Mime;
using DestilatechPlatform.API.OrdersReplenishment.Application.Services;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.ValueObjects;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Transform;
using DestilatechPlatform.API.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest;

/// <summary>
/// Replenishment orders placed with suppliers. Receiving one adds its units to stock.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Replenishment Orders")]
public class ReplenishmentOrdersController(
    IReplenishmentOrderCommandService replenishmentOrderCommandService,
    IReplenishmentOrderQueryService replenishmentOrderQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("List the replenishment orders of an account")]
    [EndpointDescription("Returns the replenishment orders of the account, newest first.")]
    [ProducesResponseType(typeof(IEnumerable<ReplenishmentOrderResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetReplenishmentOrders([FromQuery] int userId, CancellationToken cancellationToken)
    {
        if (userId <= 0)
        {
            return UserIdRequired();
        }

        var replenishmentOrders = await replenishmentOrderQueryService.Handle(
            new GetAllReplenishmentOrdersByUserIdQuery(userId), cancellationToken);
        return Ok(replenishmentOrders.Select(ReplenishmentOrderResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Get a replenishment order")]
    [ProducesResponseType(typeof(ReplenishmentOrderResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetReplenishmentOrderById(int id, [FromQuery] int userId,
        CancellationToken cancellationToken)
    {
        var replenishmentOrder = await replenishmentOrderQueryService.Handle(
            new GetReplenishmentOrderByIdQuery(id, userId), cancellationToken);
        if (replenishmentOrder is null)
        {
            return NotFound();
        }

        return Ok(ReplenishmentOrderResourceFromEntityAssembler.ToResourceFromEntity(replenishmentOrder));
    }

    [HttpPost]
    [EndpointSummary("Request a replenishment order")]
    [EndpointDescription("Registers a replenishment order in REQUESTED status. The code is generated (RP-0001, RP-0002...).")]
    [ProducesResponseType(typeof(ReplenishmentOrderResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RequestReplenishmentOrder([FromBody] CreateReplenishmentOrderResource resource,
        [FromQuery] int userId = 0, CancellationToken cancellationToken = default)
    {
        var accountId = userId > 0 ? userId : resource.UserId ?? 0;
        if (accountId <= 0)
        {
            return UserIdRequired();
        }

        var command = RequestReplenishmentOrderCommandFromResourceAssembler.ToCommandFromResource(resource, accountId);
        var result = await replenishmentOrderCommandService.Handle(command, cancellationToken);
        return result.Fold<ActionResult>(
            replenishmentOrder => CreatedAtAction(nameof(GetReplenishmentOrderById),
                new { id = replenishmentOrder.Id, userId = accountId },
                ReplenishmentOrderResourceFromEntityAssembler.ToResourceFromEntity(replenishmentOrder)),
            error => ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(error, this));
    }

    [HttpPatch("{id:int}")]
    [EndpointSummary("Receive or cancel a replenishment order")]
    [EndpointDescription("RECEIVED adds the units to stock; CANCELLED cancels it. Only a REQUESTED order can change.")]
    [ProducesResponseType(typeof(ReplenishmentOrderResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> UpdateReplenishmentOrderStatus(int id,
        [FromBody] UpdateReplenishmentOrderStatusResource resource, [FromQuery] int userId,
        CancellationToken cancellationToken)
    {
        if (userId <= 0)
        {
            return UserIdRequired();
        }

        ReplenishmentStatus requestedStatus;
        try
        {
            requestedStatus = OrdersReplenishmentValueAssembler.ToEnumFromValue<ReplenishmentStatus>(resource.Status);
        }
        catch (ArgumentException exception)
        {
            return ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(
                OrdersReplenishmentError.InvalidData(exception.Message), this);
        }

        Result<ReplenishmentOrder, OrdersReplenishmentError> result = requestedStatus switch
        {
            ReplenishmentStatus.Received => await replenishmentOrderCommandService.Handle(
                new ReceiveReplenishmentOrderCommand(id, userId), cancellationToken),
            ReplenishmentStatus.Cancelled => await replenishmentOrderCommandService.Handle(
                new CancelReplenishmentOrderCommand(id, userId), cancellationToken),
            _ => new Result<ReplenishmentOrder, OrdersReplenishmentError>.Failure(
                OrdersReplenishmentError.InvalidData("The status must be RECEIVED or CANCELLED."))
        };
        return result.Fold<ActionResult>(
            replenishmentOrder => Ok(ReplenishmentOrderResourceFromEntityAssembler.ToResourceFromEntity(replenishmentOrder)),
            error => ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(error, this));
    }

    private ActionResult UserIdRequired()
    {
        return ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(
            OrdersReplenishmentError.InvalidData("The userId of the account is required."), this);
    }
}
