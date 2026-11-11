namespace DestilatechPlatform.API.InventoryStock.Interfaces.ACL;

/// <summary>
/// Entry point of Inventory &amp; Stock for the other bounded contexts.
/// </summary>
/// <remarks>
/// The Stage methods only add the stock movement to the shared unit of work. The calling context
/// completes the unit of work, so its own change and the stock movement are saved together.
/// </remarks>
public interface IInventoryContextFacade
{
    Task<InventoryProductSnapshot?> FetchActiveProductAsync(int userId, int productId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StockLevelSnapshot>> FetchActiveStockLevelsAsync(int userId, int? productId = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StockMovementSnapshot>> FetchStockMovementsSinceAsync(int userId, DateTime sinceUtc,
        int? productId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stages the entry of bottled units produced by a batch.
    /// </summary>
    /// <returns><c>false</c> if the product is not an active bottled product of the account.</returns>
    /// <exception cref="InvalidOperationException">The entry exceeds the maximum stock.</exception>
    Task<bool> StageBottledStockAsync(int userId, int productId, int units, string reference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stages a stock movement registered by another workflow, such as an order or a replenishment.
    /// </summary>
    /// <returns><c>false</c> if the product has no stock item or the stock is not enough.</returns>
    Task<bool> StageStockMovementAsync(int userId, int productId, string type, string reason, int quantity,
        string reference, CancellationToken cancellationToken = default);
}
