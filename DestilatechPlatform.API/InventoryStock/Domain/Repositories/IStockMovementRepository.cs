using DestilatechPlatform.API.InventoryStock.Domain.Model.Entities;
using DestilatechPlatform.API.Shared.Domain.Repositories;

namespace DestilatechPlatform.API.InventoryStock.Domain.Repositories;

public interface IStockMovementRepository : IBaseRepository<StockMovement>
{
    Task<IEnumerable<StockMovement>> FindByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<IEnumerable<StockMovement>> FindByUserIdSinceAsync(int userId, DateTime sinceUtc, int? productId = null,
        CancellationToken cancellationToken = default);
}
