using DestilatechPlatform.API.InventoryStock.Domain.Model.Aggregates;
using DestilatechPlatform.API.Shared.Domain.Repositories;

namespace DestilatechPlatform.API.InventoryStock.Domain.Repositories;

public interface IStockItemRepository : IBaseRepository<StockItem>
{
    Task<IEnumerable<StockItem>> FindByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<StockItem?> FindByIdAndUserIdAsync(int stockItemId, int userId,
        CancellationToken cancellationToken = default);

    Task<StockItem?> FindByProductIdAndUserIdAsync(int productId, int userId,
        CancellationToken cancellationToken = default);
}
