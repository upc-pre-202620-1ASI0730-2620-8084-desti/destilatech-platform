using DestilatechPlatform.API.InventoryStock.Domain.Model.Aggregates;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Entities;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Queries;

namespace DestilatechPlatform.API.InventoryStock.Application.Services;

public interface IInventoryQueryService
{
    Task<IEnumerable<Product>> Handle(GetAllProductsByUserIdQuery query, CancellationToken cancellationToken = default);

    Task<IEnumerable<StockItem>> Handle(GetAllStockItemsByUserIdQuery query,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<StockMovement>> Handle(GetAllStockMovementsByUserIdQuery query,
        CancellationToken cancellationToken = default);
}
