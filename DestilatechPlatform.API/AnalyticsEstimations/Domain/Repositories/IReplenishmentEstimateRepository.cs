using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Aggregates;
using DestilatechPlatform.API.Shared.Domain.Repositories;

namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Repositories;

public interface IReplenishmentEstimateRepository : IBaseRepository<ReplenishmentEstimate>
{
    Task<ReplenishmentEstimate?> FindLatestByProductIdAsync(int userId, int productId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the most recent estimate of each product of the account.
    /// </summary>
    Task<IEnumerable<ReplenishmentEstimate>> FindLatestByUserIdAsync(int userId,
        CancellationToken cancellationToken = default);
}
