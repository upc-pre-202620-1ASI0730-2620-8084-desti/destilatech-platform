using System.Net.Mime;
using DestilatechPlatform.API.OrdersReplenishment.Application.Services;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.ValueObjects;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Transform;
using DestilatechPlatform.API.Shared.Application.Model;
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
    IReplenishmentOrderCommandService replenishmentOrderCommandService) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Request a replenishment order")]
    [EndpointDescription("Registers a replenishment order in REQUESTED status. The code is generated (RP-0001, RP-0002...).")]
    [ProducesResponseType(typeof(ReplenishmentOrderResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RequestReplenishmentOrder([FromBody] CreateReplenishmentOrderResource resource,
        CancellationToken cancellationToken = default)
    {
        var accountId = resource.TenantId;
        if (accountId <= 0)
        {
            return TenantIdRequired();
        }

        var command = RequestReplenishmentOrderCommandFromResourceAssembler.ToCommandFromResource(resource, accountId);
        var result = await replenishmentOrderCommandService.Handle(command, cancellationToken);
        return result.Match<ActionResult>(
            replenishmentOrder => CreatedAtAction(nameof(TenantReplenishmentOrdersController.GetReplenishmentOrderById),
                "TenantReplenishmentOrders", new { tenantId = accountId, replenishmentOrderId = replenishmentOrder.Id },
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
        [FromBody] UpdateReplenishmentOrderStatusResource resource, CancellationToken cancellationToken)
    {
        var userId = resource.TenantId;
        if (userId <= 0)
        {
            return TenantIdRequired();
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

        Result<ReplenishmentOrder> result = requestedStatus switch
        {
            ReplenishmentStatus.Received => await replenishmentOrderCommandService.Handle(
                new ReceiveReplenishmentOrderCommand(id, userId), cancellationToken),
            ReplenishmentStatus.Cancelled => await replenishmentOrderCommandService.Handle(
                new CancelReplenishmentOrderCommand(id, userId), cancellationToken),
            _ => Result<ReplenishmentOrder>.Failure(
                OrdersReplenishmentError.InvalidData("The status must be RECEIVED or CANCELLED."))
        };
        return result.Match<ActionResult>(
            replenishmentOrder => Ok(ReplenishmentOrderResourceFromEntityAssembler.ToResourceFromEntity(replenishmentOrder)),
            error => ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(error, this));
    }

    private ActionResult TenantIdRequired()
    {
        return ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(
            OrdersReplenishmentError.InvalidData("The tenantId is required."), this);
    }
}
