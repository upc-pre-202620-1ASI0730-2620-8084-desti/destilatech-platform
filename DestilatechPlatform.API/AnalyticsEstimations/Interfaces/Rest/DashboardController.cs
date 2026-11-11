using System.Net.Mime;
using DestilatechPlatform.API.AnalyticsEstimations.Application.Services;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Queries;
using DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Resources;
using DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest;

/// <summary>
/// Dashboard summary for the web application (US65).
/// </summary>
[ApiController]
[Route("api/v1/dashboard")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Dashboard")]
public class DashboardController(IDashboardQueryService dashboardQueryService) : ControllerBase
{
    [HttpGet("summary")]
    [EndpointSummary("Get the dashboard summary of an account")]
    [EndpointDescription("Producer: active batches, pending alerts and inventory levels. Retailer: low stock products, pending alerts and recent orders.")]
    [ProducesResponseType(typeof(DashboardSummaryResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetDashboardSummary([FromQuery] int userId, CancellationToken cancellationToken)
    {
        if (userId <= 0)
            return ActionResultFromAnalyticsEstimationsErrorAssembler.ToActionResultFromError(
                AnalyticsEstimationsError.InvalidData("The userId of the account is required."), this);

        var result = await dashboardQueryService.Handle(new GetDashboardSummaryQuery(userId), cancellationToken);
        return result.Match<ActionResult>(
            summary => Ok(DashboardSummaryResourceFromValueAssembler.ToResourceFromValue(summary)),
            error => ActionResultFromAnalyticsEstimationsErrorAssembler.ToActionResultFromError(error, this));
    }
}
