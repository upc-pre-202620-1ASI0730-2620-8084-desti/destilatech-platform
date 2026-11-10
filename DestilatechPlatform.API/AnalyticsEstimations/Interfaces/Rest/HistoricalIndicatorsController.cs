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
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Historical Indicators")]
public class HistoricalIndicatorsController(IHistoricalIndicatorQueryService historicalIndicatorQueryService)
    : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the historical indicators of an account")]
    [EndpointDescription("Groups the last days by day (up to 31), week (up to 120) or month. "
                         + "utcOffsetMinutes is the time zone of the periods; by default Peru (-300).")]
    [ProducesResponseType(typeof(HistoricalIndicatorsResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetHistoricalIndicators([FromQuery] int userId, [FromQuery] int days = 30,
        [FromQuery] int? utcOffsetMinutes = null, CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
        {
            return UserIdRequired();
        }

        var utcOffset = utcOffsetMinutes is null ? (TimeSpan?)null : TimeSpan.FromMinutes(utcOffsetMinutes.Value);
        var result = await historicalIndicatorQueryService.Handle(
            new GetHistoricalIndicatorsQuery(userId, days, utcOffset), cancellationToken);
        return result.Match<ActionResult>(
            indicators => Ok(HistoricalIndicatorsResourceFromValueAssembler.ToResourceFromValue(indicators)),
            error => ActionResultFromAnalyticsEstimationsErrorAssembler.ToActionResultFromError(error, this));
    }

    [HttpGet("top-consumed-products")]
    [EndpointSummary("Get the most consumed products of an account")]
    [EndpointDescription("Ranks the products by the units that left stock in the last days.")]
    [ProducesResponseType(typeof(IEnumerable<ProductConsumptionResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetTopConsumedProducts([FromQuery] int userId, [FromQuery] int days = 30,
        [FromQuery] int limit = 5, CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
        {
            return UserIdRequired();
        }

        var result = await historicalIndicatorQueryService.Handle(
            new GetTopConsumedProductsQuery(userId, days, limit), cancellationToken);
        return result.Match<ActionResult>(
            ranking => Ok(ranking.Select(ProductConsumptionResourceFromValueAssembler.ToResourceFromValue)),
            error => ActionResultFromAnalyticsEstimationsErrorAssembler.ToActionResultFromError(error, this));
    }

    private ActionResult UserIdRequired()
    {
        return ActionResultFromAnalyticsEstimationsErrorAssembler.ToActionResultFromError(
            AnalyticsEstimationsError.InvalidData("The userId of the account is required."), this);
    }
}
