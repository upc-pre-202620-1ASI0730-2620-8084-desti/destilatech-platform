using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.Shared.Domain.Repositories;

namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Repositories;

/// <summary>
/// Repository of customer orders. Implementations must load the order lines together
/// with the order, including in <see cref="IBaseRepository{TEntity}.FindByIdAsync"/>.
/// </summary>
public interface IOrderRepository : IBaseRepository<Order>
{
    Task<IEnumerable<Order>> FindByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Order>> FindByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the codes already used by the account, to build the code of the next order.
    /// </summary>
    Task<IEnumerable<string>> FindCodesByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}
