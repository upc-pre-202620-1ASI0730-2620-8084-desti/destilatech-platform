using System.Net.Mime;
using DestilatechPlatform.API.AnalyticsEstimations.Application.Services;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Commands;
using DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Resources;
using DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest;

/// <summary>
/// Replenishment estimate of one product (US29).
/// </summary>
[ApiController]
[Route("api/v1/products/{productId:int}/replenishment-estimate")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Replenishment Estimates")]
public class ProductReplenishmentEstimateController(
    IReplenishmentEstimateCommandService replenishmentEstimateCommandService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the replenishment estimate of a product")]
    [EndpointDescription("Calculates the estimate with the current stock and the exits of the last 30 days "
                         + "(sales, orders and waste) and keeps it in the estimate history of the account.")]
    [ProducesResponseType(typeof(ReplenishmentEstimateResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetReplenishmentEstimate(int productId, [FromQuery] int userId,
        CancellationToken cancellationToken)
    {
        if (userId <= 0)
        {
            return ActionResultFromAnalyticsEstimationsErrorAssembler.ToActionResultFromError(
                AnalyticsEstimationsError.InvalidData("The userId of the account is required."), this);
        }

        var result = await replenishmentEstimateCommandService.Handle(
            new CalculateReplenishmentEstimateCommand(userId, productId), cancellationToken);
        return result.Fold<ActionResult>(
            estimate => Ok(ReplenishmentEstimateResourceFromEntityAssembler.ToResourceFromEntity(estimate)),
            error => ActionResultFromAnalyticsEstimationsErrorAssembler.ToActionResultFromError(error, this));
    }
}
