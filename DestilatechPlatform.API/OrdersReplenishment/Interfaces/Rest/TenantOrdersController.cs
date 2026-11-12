using System.Net.Mime;
using DestilatechPlatform.API.OrdersReplenishment.Application.Services;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest;

/// <summary>
/// Orders of a tenant, as a sub-resource of the tenant: <c>/api/v1/tenants/{tenantId}/orders</c>.
/// </summary>
[ApiController]
[Route("api/v1/tenants/{tenantId:int}/orders")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Orders")]
public class TenantOrdersController(IOrderQueryService orderQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get all orders by tenant id")]
    [EndpointDescription("Returns the order history of the tenant, newest first.")]
    [ProducesResponseType(typeof(IEnumerable<OrderResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetOrdersByTenantId(int tenantId, CancellationToken cancellationToken)
    {
        var orders = await orderQueryService.Handle(new GetAllOrdersByUserIdQuery(tenantId), cancellationToken);
        return Ok(orders.Select(OrderResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{orderId:int}")]
    [EndpointSummary("Get an order of the tenant")]
    [ProducesResponseType(typeof(OrderResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetOrderById(int tenantId, int orderId, CancellationToken cancellationToken)
    {
        var order = await orderQueryService.Handle(new GetOrderByIdQuery(orderId, tenantId), cancellationToken);
        if (order is null) return NotFound();
        return Ok(OrderResourceFromEntityAssembler.ToResourceFromEntity(order));
    }
}
