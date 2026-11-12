using System.Net.Mime;
using DestilatechPlatform.API.InventoryStock.Application.Services;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Queries;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest;

/// <summary>
/// Products of a tenant, as a sub-resource of the tenant: <c>/api/v1/tenants/{tenantId}/products</c>.
/// </summary>
[ApiController]
[Route("api/v1/tenants/{tenantId:int}/products")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Inventory")]
public class TenantProductsController(IInventoryQueryService inventoryQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get all products by tenant id")]
    [ProducesResponseType(typeof(IEnumerable<ProductResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetProductsByTenantId(int tenantId, CancellationToken cancellationToken)
    {
        var products = await inventoryQueryService.Handle(new GetAllProductsByUserIdQuery(tenantId),
            cancellationToken);
        return Ok(products.Select(ProductResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
