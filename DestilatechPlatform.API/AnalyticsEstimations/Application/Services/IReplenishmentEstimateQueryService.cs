using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Aggregates;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Queries;

namespace DestilatechPlatform.API.AnalyticsEstimations.Application.Services;

public interface IReplenishmentEstimateQueryService
{
    Task<ReplenishmentEstimate?> Handle(GetLatestReplenishmentEstimateByProductIdQuery query,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ReplenishmentEstimate>> Handle(GetLatestReplenishmentEstimatesByUserIdQuery query,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ReplenishmentEstimate>> Handle(GetUrgentReplenishmentEstimatesQuery query,
        CancellationToken cancellationToken = default);
}
