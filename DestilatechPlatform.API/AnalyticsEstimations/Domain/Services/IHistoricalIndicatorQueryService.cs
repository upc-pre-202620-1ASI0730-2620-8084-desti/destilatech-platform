using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Queries;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Shared.Application.Patterns;

namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Services;

public interface IHistoricalIndicatorQueryService
{
    Task<Result<HistoricalIndicators, AnalyticsEstimationsError>> HandleAsync(GetHistoricalIndicatorsQuery query,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<ProductConsumption>, AnalyticsEstimationsError>> HandleAsync(
        GetTopConsumedProductsQuery query, CancellationToken cancellationToken = default);
}
