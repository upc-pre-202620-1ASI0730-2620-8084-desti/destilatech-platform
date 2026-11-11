using System.Net.Mime;
using DestilatechPlatform.API.InventoryStock.Application.Services;
using DestilatechPlatform.API.InventoryStock.Domain.Model;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Queries;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest;

[ApiController]
[Route("api/v1/products")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Inventory")]
public class ProductsController(
    IProductCommandService productCommandService,
    IInventoryQueryService inventoryQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("List the products of an account")]
    [ProducesResponseType(typeof(IEnumerable<ProductResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetProducts([FromQuery] int userId, CancellationToken cancellationToken)
    {
        if (userId <= 0) return UserIdRequired();

        var products = await inventoryQueryService.Handle(new GetAllProductsByUserIdQuery(userId), cancellationToken);
        return Ok(products.Select(ProductResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost]
    [EndpointSummary("Register a product")]
    [EndpointDescription("Registers a product and creates its stock item with the low stock threshold.")]
    [ProducesResponseType(typeof(ProductResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateProduct([FromQuery] int userId, [FromBody] CreateProductResource resource,
        CancellationToken cancellationToken)
    {
        if (userId <= 0) return UserIdRequired();

        var command = CreateProductCommandFromResourceAssembler.ToCommandFromResource(resource, userId);
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
    public async Task<ActionResult> UpdateProduct(int id, [FromQuery] int userId,
        [FromBody] UpdateProductResource resource, CancellationToken cancellationToken)
    {
        if (userId <= 0) return UserIdRequired();

        var command = UpdateProductCommandFromResourceAssembler.ToCommandFromResource(resource, id, userId);
        var result = await productCommandService.Handle(command, cancellationToken);
        return result.Match<ActionResult>(
            product => Ok(ProductResourceFromEntityAssembler.ToResourceFromEntity(product)),
            error => ActionResultFromInventoryStockErrorAssembler.ToActionResultFromError(error, this));
    }

    private ActionResult UserIdRequired()
    {
        return ActionResultFromInventoryStockErrorAssembler.ToActionResultFromError(
            InventoryStockError.InvalidData("The userId of the account is required."), this);
    }
}
