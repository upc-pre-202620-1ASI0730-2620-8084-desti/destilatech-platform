using DestilatechPlatform.API.InventoryStock.Application.Services;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Aggregates;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Entities;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Queries;
using DestilatechPlatform.API.InventoryStock.Domain.Repositories;

namespace DestilatechPlatform.API.InventoryStock.Application.Internal.QueryServices;

public class InventoryQueryService(
    IProductRepository productRepository,
    IStockItemRepository stockItemRepository,
    IStockMovementRepository stockMovementRepository) : IInventoryQueryService
{
    public async Task<IEnumerable<Product>> Handle(GetAllProductsByUserIdQuery query,
        CancellationToken cancellationToken = default)
    {
        return await productRepository.FindByUserIdAsync(query.UserId, cancellationToken);
    }

    public async Task<IEnumerable<StockItem>> Handle(GetAllStockItemsByUserIdQuery query,
        CancellationToken cancellationToken = default)
    {
        return await stockItemRepository.FindByUserIdAsync(query.UserId, cancellationToken);
    }

    public async Task<IEnumerable<StockMovement>> Handle(GetAllStockMovementsByUserIdQuery query,
        CancellationToken cancellationToken = default)
    {
        return await stockMovementRepository.FindByUserIdAsync(query.UserId, cancellationToken);
    }
}
