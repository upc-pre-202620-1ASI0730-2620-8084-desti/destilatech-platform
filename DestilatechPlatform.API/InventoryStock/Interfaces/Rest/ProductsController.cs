using System.Net.Mime;
using DestilatechPlatform.API.InventoryStock.Application.Services;
using DestilatechPlatform.API.InventoryStock.Domain.Model;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest;

/// <summary>
/// Write endpoints of the products. The tenant that owns the product travels in the request body;
/// the products of a tenant are listed in <see cref="TenantProductsController"/>.
/// </summary>
[ApiController]
[Route("api/v1/products")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Inventory")]
public class ProductsController(IProductCommandService productCommandService) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Register a product")]
    [EndpointDescription("Registers a product of the tenant and creates its stock item with the low stock threshold.")]
    [ProducesResponseType(typeof(ProductResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateProduct([FromBody] CreateProductResource resource,
        CancellationToken cancellationToken)
    {
        if (resource.TenantId <= 0) return TenantIdRequired();

        var command = CreateProductCommandFromResourceAssembler.ToCommandFromResource(resource, resource.TenantId);
        var result = await productCommandService.Handle(command, cancellationToken);
        return result.Match<ActionResult>(
            product => StatusCode(StatusCodes.Status201Created,
                ProductResourceFromEntityAssembler.ToResourceFromEntity(product)),
            error => ActionResultFromInventoryStockErrorAssembler.ToActionResultFromError(error, this));
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Update a product")]
    [ProducesResponseType(typeof(ProductResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateProduct(int id, [FromBody] UpdateProductResource resource,
        CancellationToken cancellationToken)
    {
        if (resource.TenantId <= 0) return TenantIdRequired();

        var command = UpdateProductCommandFromResourceAssembler.ToCommandFromResource(resource, id, resource.TenantId);
        var result = await productCommandService.Handle(command, cancellationToken);
        return result.Match<ActionResult>(
            product => Ok(ProductResourceFromEntityAssembler.ToResourceFromEntity(product)),
            error => ActionResultFromInventoryStockErrorAssembler.ToActionResultFromError(error, this));
    }

    private ActionResult TenantIdRequired()
    {
        return ActionResultFromInventoryStockErrorAssembler.ToActionResultFromError(
            InventoryStockError.InvalidData("The tenantId is required."), this);
    }
}
