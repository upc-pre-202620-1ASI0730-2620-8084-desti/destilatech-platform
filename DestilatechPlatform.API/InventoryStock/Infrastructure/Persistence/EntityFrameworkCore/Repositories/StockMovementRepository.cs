using DestilatechPlatform.API.InventoryStock.Domain.Model.Entities;
using DestilatechPlatform.API.InventoryStock.Domain.Repositories;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.InventoryStock.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class StockMovementRepository(AppDbContext context)
    : BaseRepository<StockMovement>(context), IStockMovementRepository
{
    public async Task<IEnumerable<StockMovement>> FindByUserIdAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<StockMovement>()
            .AsNoTracking()
            .Where(movement => movement.UserId == userId)
            .OrderByDescending(movement => movement.MovementDate)
            .ThenByDescending(movement => movement.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockMovement>> FindByUserIdSinceAsync(int userId, DateTime sinceUtc,
        int? productId = null, CancellationToken cancellationToken = default)
    {
        var movements = Context.Set<StockMovement>()
            .AsNoTracking()
            .Where(movement => movement.UserId == userId && movement.MovementDate >= sinceUtc);
        if (productId is not null)
            movements = movements.Where(movement => movement.ProductId == productId);
        return await movements.OrderBy(movement => movement.MovementDate).ToListAsync(cancellationToken);
    }
}
