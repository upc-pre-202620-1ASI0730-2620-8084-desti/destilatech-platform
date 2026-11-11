using DestilatechPlatform.API.InventoryStock.Domain.Model.Aggregates;
using DestilatechPlatform.API.InventoryStock.Domain.Repositories;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.InventoryStock.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class StockItemRepository(AppDbContext context) : BaseRepository<StockItem>(context), IStockItemRepository
{
    public async Task<IEnumerable<StockItem>> FindByUserIdAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<StockItem>()
            .Where(stockItem => stockItem.UserId == userId)
            .OrderBy(stockItem => stockItem.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<StockItem?> FindByIdAndUserIdAsync(int stockItemId, int userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<StockItem>()
            .FirstOrDefaultAsync(stockItem => stockItem.Id == stockItemId && stockItem.UserId == userId,
                cancellationToken);
    }

    public async Task<StockItem?> FindByProductIdAndUserIdAsync(int productId, int userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<StockItem>()
            .FirstOrDefaultAsync(stockItem => stockItem.ProductId == productId && stockItem.UserId == userId,
                cancellationToken);
    }
}
