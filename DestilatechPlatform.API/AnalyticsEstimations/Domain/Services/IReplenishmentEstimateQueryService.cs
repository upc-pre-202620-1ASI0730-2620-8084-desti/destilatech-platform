using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Aggregates;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Queries;

namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Services;

public interface IReplenishmentEstimateQueryService
{
    Task<ReplenishmentEstimate?> HandleAsync(GetLatestReplenishmentEstimateByProductIdQuery query,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ReplenishmentEstimate>> HandleAsync(GetLatestReplenishmentEstimatesByUserIdQuery query,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ReplenishmentEstimate>> HandleAsync(GetUrgentReplenishmentEstimatesQuery query,
        CancellationToken cancellationToken = default);
}
