using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Queries;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Shared.Application.Model;

namespace DestilatechPlatform.API.AnalyticsEstimations.Application.Services;

public interface IHistoricalIndicatorQueryService
{
    Task<Result<HistoricalIndicators>> Handle(GetHistoricalIndicatorsQuery query,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<ProductConsumption>>> Handle(
        GetTopConsumedProductsQuery query, CancellationToken cancellationToken = default);
}
