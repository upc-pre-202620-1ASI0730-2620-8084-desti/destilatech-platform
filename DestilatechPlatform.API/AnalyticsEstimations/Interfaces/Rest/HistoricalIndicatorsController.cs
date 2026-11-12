using System.Net.Mime;
using DestilatechPlatform.API.AnalyticsEstimations.Application.Services;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Queries;
using DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Resources;
using DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest;

/// <summary>
/// Historical indicators of an account (US30): stock level, entries, exits, sales and production.
/// </summary>
[ApiController]
[Route("api/v1/tenants/{tenantId:int}/historical-indicators")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Historical Indicators")]
public class HistoricalIndicatorsController(IHistoricalIndicatorQueryService historicalIndicatorQueryService)
    : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the historical indicators by tenant id")]
    [EndpointDescription("Groups the last days by day (up to 31), week (up to 120) or month. "
                         + "utcOffsetMinutes is the time zone of the periods; by default Peru (-300).")]
    [ProducesResponseType(typeof(HistoricalIndicatorsResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetHistoricalIndicators(int tenantId, [FromQuery] int days = 30,
        [FromQuery] int? utcOffsetMinutes = null, CancellationToken cancellationToken = default)
    {

        var utcOffset = utcOffsetMinutes is null ? (TimeSpan?)null : TimeSpan.FromMinutes(utcOffsetMinutes.Value);
        var result = await historicalIndicatorQueryService.Handle(
            new GetHistoricalIndicatorsQuery(tenantId, days, utcOffset), cancellationToken);
        return result.Match<ActionResult>(
            indicators => Ok(HistoricalIndicatorsResourceFromValueAssembler.ToResourceFromValue(indicators)),
            error => ActionResultFromAnalyticsEstimationsErrorAssembler.ToActionResultFromError(error, this));
    }

    [HttpGet("top-consumed-products")]
    [EndpointSummary("Get the most consumed products by tenant id")]
    [EndpointDescription("Ranks the products by the units that left stock in the last days.")]
    [ProducesResponseType(typeof(IEnumerable<ProductConsumptionResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetTopConsumedProducts(int tenantId, [FromQuery] int days = 30,
        [FromQuery] int limit = 5, CancellationToken cancellationToken = default)
    {

        var result = await historicalIndicatorQueryService.Handle(
            new GetTopConsumedProductsQuery(tenantId, days, limit), cancellationToken);
        return result.Match<ActionResult>(
            ranking => Ok(ranking.Select(ProductConsumptionResourceFromValueAssembler.ToResourceFromValue)),
            error => ActionResultFromAnalyticsEstimationsErrorAssembler.ToActionResultFromError(error, this));
    }
}
