using System.Net.Mime;
using DestilatechPlatform.API.InventoryStock.Application.Services;
using DestilatechPlatform.API.InventoryStock.Domain.Model;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Queries;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest;

[ApiController]
[Route("api/v1/stock-items")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Inventory")]
public class StockItemsController(
    IStockItemCommandService stockItemCommandService,
    IInventoryQueryService inventoryQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("List the stock items of an account")]
    [ProducesResponseType(typeof(IEnumerable<StockItemResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetStockItems([FromQuery] int userId, CancellationToken cancellationToken)
    {
        if (userId <= 0) return UserIdRequired();

        var stockItems = await inventoryQueryService.Handle(new GetAllStockItemsByUserIdQuery(userId),
            cancellationToken);
        return Ok(stockItems.Select(StockItemResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPatch("{id:int}")]
    [EndpointSummary("Configure the low stock threshold")]
    [EndpointDescription("Updates the threshold and raises a low stock alert if the stock is at or below it.")]
    [ProducesResponseType(typeof(StockItemResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> ConfigureLowStockThreshold(int id, [FromQuery] int userId,
        [FromBody] ConfigureLowStockThresholdResource resource, CancellationToken cancellationToken)
    {
        if (userId <= 0) return UserIdRequired();

        var command = ConfigureLowStockThresholdCommandFromResourceAssembler.ToCommandFromResource(resource, id, userId);
        var result = await stockItemCommandService.Handle(command, cancellationToken);
        return result.Match<ActionResult>(
            stockItem => Ok(StockItemResourceFromEntityAssembler.ToResourceFromEntity(stockItem)),
            error => ActionResultFromInventoryStockErrorAssembler.ToActionResultFromError(error, this));
    }

    private ActionResult UserIdRequired()
    {
        return ActionResultFromInventoryStockErrorAssembler.ToActionResultFromError(
            InventoryStockError.InvalidData("The userId of the account is required."), this);
    }
}
