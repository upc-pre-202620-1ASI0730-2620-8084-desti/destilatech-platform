using System.Net.Mime;
using DestilatechPlatform.API.InventoryStock.Application.Services;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Queries;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest;

/// <summary>
/// Stock items of a tenant, as a sub-resource of the tenant: <c>/api/v1/tenants/{tenantId}/stock-items</c>.
/// </summary>
[ApiController]
[Route("api/v1/tenants/{tenantId:int}/stock-items")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Inventory")]
public class TenantStockItemsController(IInventoryQueryService inventoryQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get all stock items by tenant id")]
    [ProducesResponseType(typeof(IEnumerable<StockItemResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetStockItemsByTenantId(int tenantId, CancellationToken cancellationToken)
    {
        var stockItems = await inventoryQueryService.Handle(new GetAllStockItemsByUserIdQuery(tenantId),
            cancellationToken);
        return Ok(stockItems.Select(StockItemResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
