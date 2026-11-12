using System.Net.Mime;
using DestilatechPlatform.API.AnalyticsEstimations.Application.Services;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Commands;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Queries;
using DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Resources;
using DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest;

/// <summary>
/// Replenishment estimates of all the products of an account, used by the priority replenishment panel.
/// </summary>
[ApiController]
[Route("api/v1/tenants/{tenantId:int}/replenishment-estimates")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Replenishment Estimates")]
public class ReplenishmentEstimatesController(
    IReplenishmentEstimateCommandService replenishmentEstimateCommandService,
    IReplenishmentEstimateQueryService replenishmentEstimateQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the latest replenishment estimates by tenant id")]
    [EndpointDescription("Returns the latest estimate of each product. With urgentOnly=true it only returns "
                         + "the products that reach their threshold in seven days or less, the closest first.")]
    [ProducesResponseType(typeof(IEnumerable<ReplenishmentEstimateResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetReplenishmentEstimates(int tenantId,
        [FromQuery] bool urgentOnly = false, CancellationToken cancellationToken = default)
    {

        var estimates = urgentOnly
            ? await replenishmentEstimateQueryService.Handle(new GetUrgentReplenishmentEstimatesQuery(tenantId),
                cancellationToken)
            : await replenishmentEstimateQueryService.Handle(new GetLatestReplenishmentEstimatesByUserIdQuery(tenantId),
                cancellationToken);
        return Ok(estimates.Select(ReplenishmentEstimateResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost("recalculations")]
    [EndpointSummary("Recalculate the replenishment estimates of a tenant")]
    [EndpointDescription("Calculates a new estimate for every active product of the account and returns them.")]
    [ProducesResponseType(typeof(IEnumerable<ReplenishmentEstimateResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RecalculateReplenishmentEstimates(int tenantId,
        CancellationToken cancellationToken)
    {

        var result = await replenishmentEstimateCommandService.Handle(
            new RecalculateReplenishmentEstimatesCommand(tenantId), cancellationToken);
        return result.Match<ActionResult>(
            estimates => Ok(estimates.Select(ReplenishmentEstimateResourceFromEntityAssembler.ToResourceFromEntity)),
            error => ActionResultFromAnalyticsEstimationsErrorAssembler.ToActionResultFromError(error, this));
    }
}
