using System.Net.Mime;
using DestilatechPlatform.API.InventoryStock.Application.Services;
using DestilatechPlatform.API.InventoryStock.Domain.Model;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest;

/// <summary>
/// Write endpoint of the stock movements. The movements of a tenant are listed in
/// <see cref="TenantStockMovementsController"/>.
/// </summary>
[ApiController]
[Route("api/v1/stock-movements")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Inventory")]
public class StockMovementsController(IStockItemCommandService stockItemCommandService) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Register a manual stock movement")]
    [EndpointDescription("Registers a stock entry or exit. Production, order and return movements are registered by their own workflows.")]
    [ProducesResponseType(typeof(StockMovementResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RegisterStockMovement([FromBody] RegisterStockMovementResource resource,
        CancellationToken cancellationToken)
    {
        if (resource.TenantId <= 0)
            return ActionResultFromInventoryStockErrorAssembler.ToActionResultFromError(
                InventoryStockError.InvalidData("The tenantId is required."), this);

        var command = RegisterStockMovementCommandFromResourceAssembler.ToCommandFromResource(resource,
            resource.TenantId);
        var result = await stockItemCommandService.Handle(command, cancellationToken);
        return result.Match<ActionResult>(
            movement => StatusCode(StatusCodes.Status201Created,
                StockMovementResourceFromEntityAssembler.ToResourceFromEntity(movement)),
            error => ActionResultFromInventoryStockErrorAssembler.ToActionResultFromError(error, this));
    }
}
