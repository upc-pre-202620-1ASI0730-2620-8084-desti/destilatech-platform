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
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Replenishment Estimates")]
public class ReplenishmentEstimatesController(
    IReplenishmentEstimateCommandService replenishmentEstimateCommandService,
    IReplenishmentEstimateQueryService replenishmentEstimateQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("List the latest replenishment estimates of an account")]
    [EndpointDescription("Returns the latest estimate of each product. With urgentOnly=true it only returns "
                         + "the products that reach their threshold in seven days or less, the closest first.")]
    [ProducesResponseType(typeof(IEnumerable<ReplenishmentEstimateResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetReplenishmentEstimates([FromQuery] int userId,
        [FromQuery] bool urgentOnly = false, CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
        {
            return UserIdRequired();
        }

        var estimates = urgentOnly
            ? await replenishmentEstimateQueryService.Handle(new GetUrgentReplenishmentEstimatesQuery(userId),
                cancellationToken)
            : await replenishmentEstimateQueryService.Handle(new GetLatestReplenishmentEstimatesByUserIdQuery(userId),
                cancellationToken);
        return Ok(estimates.Select(ReplenishmentEstimateResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost("recalculations")]
    [EndpointSummary("Recalculate the replenishment estimates of an account")]
    [EndpointDescription("Calculates a new estimate for every active product of the account and returns them.")]
    [ProducesResponseType(typeof(IEnumerable<ReplenishmentEstimateResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RecalculateReplenishmentEstimates([FromQuery] int userId,
        CancellationToken cancellationToken)
    {
        if (userId <= 0)
        {
            return UserIdRequired();
        }

        var result = await replenishmentEstimateCommandService.Handle(
            new RecalculateReplenishmentEstimatesCommand(userId), cancellationToken);
        return result.Fold<ActionResult>(
            estimates => Ok(estimates.Select(ReplenishmentEstimateResourceFromEntityAssembler.ToResourceFromEntity)),
            error => ActionResultFromAnalyticsEstimationsErrorAssembler.ToActionResultFromError(error, this));
    }

    private ActionResult UserIdRequired()
    {
        return ActionResultFromAnalyticsEstimationsErrorAssembler.ToActionResultFromError(
            AnalyticsEstimationsError.InvalidData("The userId of the account is required."), this);
    }
}
