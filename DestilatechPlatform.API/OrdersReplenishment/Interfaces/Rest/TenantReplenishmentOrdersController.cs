using System.Net.Mime;
using DestilatechPlatform.API.OrdersReplenishment.Application.Services;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest;

/// <summary>
/// Replenishment orders of a tenant, as a sub-resource of the tenant:
/// <c>/api/v1/tenants/{tenantId}/replenishment-orders</c>.
/// </summary>
[ApiController]
[Route("api/v1/tenants/{tenantId:int}/replenishment-orders")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Replenishment Orders")]
public class TenantReplenishmentOrdersController(IReplenishmentOrderQueryService replenishmentOrderQueryService)
    : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get all replenishment orders by tenant id")]
    [EndpointDescription("Returns the replenishment orders of the tenant, newest first.")]
    [ProducesResponseType(typeof(IEnumerable<ReplenishmentOrderResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetReplenishmentOrdersByTenantId(int tenantId, CancellationToken cancellationToken)
    {
        var replenishmentOrders = await replenishmentOrderQueryService.Handle(
            new GetAllReplenishmentOrdersByUserIdQuery(tenantId), cancellationToken);
        return Ok(replenishmentOrders.Select(ReplenishmentOrderResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{replenishmentOrderId:int}")]
    [EndpointSummary("Get a replenishment order of the tenant")]
    [ProducesResponseType(typeof(ReplenishmentOrderResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetReplenishmentOrderById(int tenantId, int replenishmentOrderId,
        CancellationToken cancellationToken)
    {
        var replenishmentOrder = await replenishmentOrderQueryService.Handle(
            new GetReplenishmentOrderByIdQuery(replenishmentOrderId, tenantId), cancellationToken);
        if (replenishmentOrder is null) return NotFound();
        return Ok(ReplenishmentOrderResourceFromEntityAssembler.ToResourceFromEntity(replenishmentOrder));
    }
}
