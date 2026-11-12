using System.Net.Mime;
using DestilatechPlatform.API.InventoryStock.Application.Services;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Queries;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest;

/// <summary>
/// Stock movements of a tenant, as a sub-resource of the tenant:
/// <c>/api/v1/tenants/{tenantId}/stock-movements</c>.
/// </summary>
[ApiController]
[Route("api/v1/tenants/{tenantId:int}/stock-movements")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Inventory")]
public class TenantStockMovementsController(IInventoryQueryService inventoryQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get all stock movements by tenant id")]
    [ProducesResponseType(typeof(IEnumerable<StockMovementResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetStockMovementsByTenantId(int tenantId, CancellationToken cancellationToken)
    {
        var movements = await inventoryQueryService.Handle(new GetAllStockMovementsByUserIdQuery(tenantId),
            cancellationToken);
        return Ok(movements.Select(StockMovementResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
