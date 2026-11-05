using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Aggregates;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Queries;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Repositories;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Services;

namespace DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.QueryServices;

public class ReplenishmentEstimateQueryService(IReplenishmentEstimateRepository estimateRepository)
    : IReplenishmentEstimateQueryService
{
    public async Task<ReplenishmentEstimate?> HandleAsync(GetLatestReplenishmentEstimateByProductIdQuery query,
        CancellationToken cancellationToken = default)
    {
        return await estimateRepository.FindLatestByProductIdAsync(query.UserId, query.ProductId, cancellationToken);
    }

    public async Task<IEnumerable<ReplenishmentEstimate>> HandleAsync(GetLatestReplenishmentEstimatesByUserIdQuery query,
        CancellationToken cancellationToken = default)
    {
        return await estimateRepository.FindLatestByUserIdAsync(query.UserId, cancellationToken);
    }

    public async Task<IEnumerable<ReplenishmentEstimate>> HandleAsync(GetUrgentReplenishmentEstimatesQuery query,
        CancellationToken cancellationToken = default)
    {
        var estimates = await estimateRepository.FindLatestByUserIdAsync(query.UserId, cancellationToken);
        return estimates
            .Where(estimate => estimate.IsUrgent())
            .OrderBy(estimate => estimate.DaysUntilThreshold);
    }
}
