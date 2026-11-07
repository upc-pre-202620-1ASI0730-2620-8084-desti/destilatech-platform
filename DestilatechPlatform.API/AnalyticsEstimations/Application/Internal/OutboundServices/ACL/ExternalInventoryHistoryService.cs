using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;
using DestilatechPlatform.API.InventoryStock.Application;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.OutboundServices.ACL;

/// <summary>
/// Adapter of <see cref="IInventoryHistoryReadPort"/> over the Inventory &amp; Stock Management bounded context.
/// It only reads stock items and movements; Analytics never changes the inventory.
/// </summary>
public class ExternalInventoryHistoryService(InventoryService inventoryService) : IInventoryHistoryReadPort
{
    private const string MovementIn = "IN";

    public async Task<StockLevel?> ReadStockLevelAsync(int userId, int productId,
        CancellationToken cancellationToken = default)
    {
        var levels = await ReadActiveStockLevelsAsync(userId, productId, cancellationToken);
        return levels.Count == 0 ? null : levels[0];
    }

    public async Task<IReadOnlyCollection<StockLevel>> ReadStockLevelsAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        return await ReadActiveStockLevelsAsync(userId, null, cancellationToken);
    }

    public async Task<IReadOnlyCollection<StockMovementRecord>> ReadMovementsAsync(int userId, DateTimeOffset since,
        CancellationToken cancellationToken = default)
    {
        return await ReadMovementRecordsAsync(userId, null, since, cancellationToken);
    }

    public async Task<IReadOnlyCollection<StockMovementRecord>> ReadProductMovementsAsync(int userId, int productId,
        DateTimeOffset since, CancellationToken cancellationToken = default)
    {
        return await ReadMovementRecordsAsync(userId, productId, since, cancellationToken);
    }

    private async Task<List<StockLevel>> ReadActiveStockLevelsAsync(int userId, int? productId,
        CancellationToken cancellationToken)
    {
        var activeProductIds = inventoryService.Products(userId)
            .Where(product => product.IsActive)
            .Select(product => product.Id);
        var stockItems = inventoryService.StockItems(userId)
            .Where(stock => activeProductIds.Contains(stock.ProductId));
        if (productId is not null)
        {
            stockItems = stockItems.Where(stock => stock.ProductId == productId);
        }

        var rows = await stockItems
            .OrderBy(stock => stock.ProductId)
            .Select(stock => new { stock.ProductId, stock.CurrentQuantity, stock.LowStockThreshold })
            .ToListAsync(cancellationToken);
        return rows
            .Select(row => new StockLevel(row.ProductId, row.CurrentQuantity, row.LowStockThreshold))
            .ToList();
    }

    private async Task<List<StockMovementRecord>> ReadMovementRecordsAsync(int userId, int? productId,
        DateTimeOffset since, CancellationToken cancellationToken)
    {
        // Inventory keeps movement dates as UTC DateTime values.
        var sinceUtc = since.UtcDateTime;
        var movements = inventoryService.Movements(userId)
            .Where(movement => movement.MovementDate >= sinceUtc);
        if (productId is not null)
        {
            movements = movements.Where(movement => movement.ProductId == productId);
        }

        var rows = await movements
            .OrderBy(movement => movement.MovementDate)
            .Select(movement => new
            {
                movement.ProductId, movement.Type, movement.Reason, movement.Quantity, movement.MovementDate
            })
            .ToListAsync(cancellationToken);
        return rows
            .Select(row => new StockMovementRecord(
                row.ProductId,
                row.Type == MovementIn ? MovementDirection.In : MovementDirection.Out,
                row.Reason,
                row.Quantity,
                new DateTimeOffset(DateTime.SpecifyKind(row.MovementDate, DateTimeKind.Utc))))
            .ToList();
    }
}
