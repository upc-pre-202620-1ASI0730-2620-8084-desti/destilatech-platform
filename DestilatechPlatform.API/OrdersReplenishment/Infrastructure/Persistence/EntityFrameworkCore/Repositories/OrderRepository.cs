using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Repositories;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.OrdersReplenishment.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
/// Repository of orders. The lines are loaded automatically with each order (see the model configuration).
/// </summary>
public class OrderRepository(AppDbContext context) : BaseRepository<Order>(context), IOrderRepository
{
    public async Task<IEnumerable<Order>> FindByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Order>()
            .Where(order => order.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Order>> FindByCustomerIdAsync(int customerId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Order>()
            .Where(order => order.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> FindCodesByUserIdAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Order>()
            .IgnoreAutoIncludes()
            .Where(order => order.UserId == userId)
            .Select(order => order.Code)
            .ToListAsync(cancellationToken);
    }
}
