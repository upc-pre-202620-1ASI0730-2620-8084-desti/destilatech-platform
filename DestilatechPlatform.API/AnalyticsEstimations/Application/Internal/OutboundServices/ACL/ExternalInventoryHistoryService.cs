using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;
using DestilatechPlatform.API.InventoryStock.Interfaces.ACL;

namespace DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.OutboundServices.ACL;

/// <summary>
/// Adapter of <see cref="IInventoryHistoryReadPort"/> over the Inventory &amp; Stock Management bounded context.
/// It only reads stock items and movements; Analytics never changes the inventory.
/// </summary>
public class ExternalInventoryHistoryService(IInventoryContextFacade inventoryContextFacade) : IInventoryHistoryReadPort
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
        var levels = await inventoryContextFacade.FetchActiveStockLevelsAsync(userId, productId, cancellationToken);
        return levels
            .Select(level => new StockLevel(level.ProductId, level.CurrentQuantity, level.LowStockThreshold))
            .ToList();
    }

    private async Task<List<StockMovementRecord>> ReadMovementRecordsAsync(int userId, int? productId,
        DateTimeOffset since, CancellationToken cancellationToken)
    {
        // Inventory keeps movement dates as UTC DateTime values.
        var movements = await inventoryContextFacade.FetchStockMovementsSinceAsync(userId, since.UtcDateTime,
            productId, cancellationToken);
        return movements
            .Select(movement => new StockMovementRecord(
                movement.ProductId,
                movement.Type == MovementIn ? MovementDirection.In : MovementDirection.Out,
                movement.Reason,
                movement.Quantity,
                new DateTimeOffset(DateTime.SpecifyKind(movement.MovementDate, DateTimeKind.Utc))))
            .ToList();
    }
}
