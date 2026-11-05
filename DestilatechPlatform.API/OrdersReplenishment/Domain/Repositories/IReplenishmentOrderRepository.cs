using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.Shared.Domain.Repositories;

namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Repositories;

public interface IReplenishmentOrderRepository : IBaseRepository<ReplenishmentOrder>
{
    Task<IEnumerable<ReplenishmentOrder>> FindByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts the replenishment orders of the account, to build the code of the next one.
    /// </summary>
    Task<int> CountByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}
