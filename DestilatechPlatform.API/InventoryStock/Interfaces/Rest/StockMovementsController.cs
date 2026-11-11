using System.Net.Mime;
using DestilatechPlatform.API.InventoryStock.Application.Services;
using DestilatechPlatform.API.InventoryStock.Domain.Model;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Queries;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest;

[ApiController]
[Route("api/v1/stock-movements")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Inventory")]
public class StockMovementsController(
    IStockItemCommandService stockItemCommandService,
    IInventoryQueryService inventoryQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("List the stock movements of an account")]
    [ProducesResponseType(typeof(IEnumerable<StockMovementResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetStockMovements([FromQuery] int userId, CancellationToken cancellationToken)
    {
        if (userId <= 0) return UserIdRequired();

        var movements = await inventoryQueryService.Handle(new GetAllStockMovementsByUserIdQuery(userId),
            cancellationToken);
        return Ok(movements.Select(StockMovementResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost]
    [EndpointSummary("Register a manual stock movement")]
    [EndpointDescription("Registers a stock entry or exit. Production, order and return movements are registered by their own workflows.")]
    [ProducesResponseType(typeof(StockMovementResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RegisterStockMovement([FromQuery] int userId,
        [FromBody] RegisterStockMovementResource resource, CancellationToken cancellationToken)
    {
        if (userId <= 0) return UserIdRequired();

        var command = RegisterStockMovementCommandFromResourceAssembler.ToCommandFromResource(resource, userId);
        var result = await stockItemCommandService.Handle(command, cancellationToken);
        return result.Match<ActionResult>(
            movement => StatusCode(StatusCodes.Status201Created,
                StockMovementResourceFromEntityAssembler.ToResourceFromEntity(movement)),
            error => ActionResultFromInventoryStockErrorAssembler.ToActionResultFromError(error, this));
    }

    private ActionResult UserIdRequired()
    {
        return ActionResultFromInventoryStockErrorAssembler.ToActionResultFromError(
            InventoryStockError.InvalidData("The userId of the account is required."), this);
    }
}
