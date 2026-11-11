using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;

namespace DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.OutboundServices;

/// <summary>
/// Read-only outbound port towards Inventory &amp; Stock Management. Analytics &amp; Estimations reads
/// stock levels and movements through this port instead of using the inventory model directly.
/// The adapter will be implemented once the Inventory &amp; Stock bounded context exists.
/// </summary>
public interface IInventoryHistoryReadPort
{
    /// <returns>The stock level of the product, or <c>null</c> if the product does not exist for the account.</returns>
    Task<StockLevel?> ReadStockLevelAsync(int userId, int productId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StockLevel>> ReadStockLevelsAsync(int userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads the stock movements of every product of the account since a date.
    /// </summary>
    Task<IReadOnlyCollection<StockMovementRecord>> ReadMovementsAsync(int userId, DateTimeOffset since,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads the stock movements of one product since a date.
    /// </summary>
    Task<IReadOnlyCollection<StockMovementRecord>> ReadProductMovementsAsync(int userId, int productId,
        DateTimeOffset since, CancellationToken cancellationToken = default);
}
