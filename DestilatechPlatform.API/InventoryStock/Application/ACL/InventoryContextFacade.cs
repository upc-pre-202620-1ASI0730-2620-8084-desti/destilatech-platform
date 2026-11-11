using DestilatechPlatform.API.InventoryStock.Domain.Repositories;
using DestilatechPlatform.API.InventoryStock.Interfaces.ACL;

namespace DestilatechPlatform.API.InventoryStock.Application.ACL;

public class InventoryContextFacade(
    IProductRepository productRepository,
    IStockItemRepository stockItemRepository,
    IStockMovementRepository stockMovementRepository) : IInventoryContextFacade
{
    public async Task<InventoryProductSnapshot?> FetchActiveProductAsync(int userId, int productId,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.FindByIdAndUserIdAsync(productId, userId, cancellationToken);
        if (product is not { IsActive: true }) return null;

        var stockItem = await stockItemRepository.FindByProductIdAndUserIdAsync(productId, userId, cancellationToken);
        return new InventoryProductSnapshot(product.Id, product.Name, product.Presentation, product.UnitPrice,
            stockItem?.CurrentQuantity ?? 0m);
    }

    public async Task<IReadOnlyCollection<StockLevelSnapshot>> FetchActiveStockLevelsAsync(int userId,
        int? productId = null, CancellationToken cancellationToken = default)
    {
        var activeProductIds = (await productRepository.FindByUserIdAsync(userId, cancellationToken))
            .Where(product => product.IsActive)
            .Select(product => product.Id)
            .ToHashSet();
        var stockItems = await stockItemRepository.FindByUserIdAsync(userId, cancellationToken);
        return stockItems
            .Where(stockItem => activeProductIds.Contains(stockItem.ProductId))
            .Where(stockItem => productId is null || stockItem.ProductId == productId)
            .OrderBy(stockItem => stockItem.ProductId)
            .Select(stockItem => new StockLevelSnapshot(stockItem.ProductId, stockItem.CurrentQuantity,
                stockItem.LowStockThreshold))
            .ToList();
    }

    public async Task<IReadOnlyCollection<StockOverviewSnapshot>> FetchStockOverviewAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        var products = (await productRepository.FindByUserIdAsync(userId, cancellationToken))
            .Where(product => product.IsActive)
            .ToDictionary(product => product.Id);
        var stockItems = await stockItemRepository.FindByUserIdAsync(userId, cancellationToken);
        return stockItems
            .Where(stockItem => products.ContainsKey(stockItem.ProductId))
            .OrderBy(stockItem => stockItem.ProductId)
            .Select(stockItem => new StockOverviewSnapshot(stockItem.ProductId, products[stockItem.ProductId].Name,
                products[stockItem.ProductId].Presentation, stockItem.CurrentQuantity, stockItem.LowStockThreshold,
                stockItem.IsBelowThreshold()))
            .ToList();
    }

    public async Task<IReadOnlyCollection<StockMovementSnapshot>> FetchStockMovementsSinceAsync(int userId,
        DateTime sinceUtc, int? productId = null, CancellationToken cancellationToken = default)
    {
        var movements = await stockMovementRepository.FindByUserIdSinceAsync(userId, sinceUtc, productId,
            cancellationToken);
        return movements
            .Select(movement => new StockMovementSnapshot(movement.ProductId, movement.Type, movement.Reason,
                movement.Quantity, movement.MovementDate))
            .ToList();
    }

    public async Task<bool> StageBottledStockAsync(int userId, int productId, int units, string reference,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.FindByIdAndUserIdAsync(productId, userId, cancellationToken);
        if (product is not { IsActive: true, Unit: "BOTTLE" }) return false;

        var stockItem = await stockItemRepository.FindByProductIdAndUserIdAsync(productId, userId, cancellationToken);
        if (stockItem is null) return false;

        await stockMovementRepository.AddAsync(
            stockItem.RegisterMovement("IN", "PRODUCTION", units, reference: reference), cancellationToken);
        return true;
    }

    public async Task<bool> StageStockMovementAsync(int userId, int productId, string type, string reason,
        int quantity, string reference, CancellationToken cancellationToken = default)
    {
        var stockItem = await stockItemRepository.FindByProductIdAndUserIdAsync(productId, userId, cancellationToken);
        if (stockItem is null) return false;

        try
        {
            await stockMovementRepository.AddAsync(
                stockItem.RegisterMovement(type, reason, quantity, reference: reference), cancellationToken);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}
