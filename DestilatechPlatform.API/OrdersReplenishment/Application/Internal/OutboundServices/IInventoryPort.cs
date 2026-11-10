namespace DestilatechPlatform.API.OrdersReplenishment.Application.Internal.OutboundServices;

/// <summary>
/// Outbound port towards Inventory &amp; Stock Management. Orders &amp; Replenishment reads products
/// and registers stock movements through this port, without touching the StockItem aggregate.
/// </summary>
/// <remarks>
/// The adapter will be implemented once the Inventory &amp; Stock bounded context exists.
/// It must only add the stock movements to the shared unit of work, without saving them:
/// the command services complete the unit of work, so an order and its stock movements
/// are saved in the same transaction.
/// </remarks>
public interface IInventoryPort
{
    /// <summary>
    /// Finds a product of the account with its current price and stock.
    /// </summary>
    /// <returns>The product, or <c>null</c> if it does not exist for the account.</returns>
    Task<InventoryProduct?> FindProductAsync(int userId, int productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a stock exit with reason ORDER for a product sold in an order.
    /// </summary>
    /// <param name="reference">Code of the order, for example <c>#1037</c>.</param>
    Task RegisterOrderExitAsync(int userId, int productId, int quantity, string reference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a stock entry for a cancelled order or a received replenishment order.
    /// </summary>
    /// <param name="reference">Code of the order or replenishment order.</param>
    Task RegisterEntryAsync(int userId, int productId, int quantity, StockEntryReason reason, string reference,
        CancellationToken cancellationToken = default);
}
