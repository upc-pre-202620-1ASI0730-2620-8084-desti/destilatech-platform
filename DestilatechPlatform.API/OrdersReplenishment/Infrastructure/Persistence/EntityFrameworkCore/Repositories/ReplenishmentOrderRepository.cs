using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Repositories;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.OrdersReplenishment.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class ReplenishmentOrderRepository(AppDbContext context)
    : BaseRepository<ReplenishmentOrder>(context), IReplenishmentOrderRepository
{
    public async Task<IEnumerable<ReplenishmentOrder>> FindByUserIdAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<ReplenishmentOrder>()
            .Where(replenishmentOrder => replenishmentOrder.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<ReplenishmentOrder>()
            .CountAsync(replenishmentOrder => replenishmentOrder.UserId == userId, cancellationToken);
    }
}
