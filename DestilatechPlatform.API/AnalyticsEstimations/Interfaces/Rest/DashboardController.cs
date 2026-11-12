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
[Route("api/v1/tenants/{tenantId:int}/dashboard-summary")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Dashboard")]
public class DashboardController(IDashboardQueryService dashboardQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the dashboard summary by tenant id")]
    [EndpointDescription("Producer: active batches, pending alerts and inventory levels. Retailer: low stock products, pending alerts and recent orders.")]
    [ProducesResponseType(typeof(DashboardSummaryResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetDashboardSummary(int tenantId, CancellationToken cancellationToken)
    {
        var result = await dashboardQueryService.Handle(new GetDashboardSummaryQuery(tenantId), cancellationToken);
        return result.Match<ActionResult>(
            summary => Ok(DashboardSummaryResourceFromValueAssembler.ToResourceFromValue(summary)),
            error => ActionResultFromAnalyticsEstimationsErrorAssembler.ToActionResultFromError(error, this));
    }
}
