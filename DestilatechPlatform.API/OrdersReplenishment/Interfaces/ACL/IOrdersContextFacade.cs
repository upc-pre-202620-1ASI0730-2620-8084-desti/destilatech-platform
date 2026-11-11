namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.ACL;

/// <summary>
/// Order data that other bounded contexts can read, for example the sales volume of Analytics &amp; Estimations.
/// </summary>
/// <param name="OrderId">Order identifier.</param>
/// <param name="OrderDate">When the order was placed.</param>
/// <param name="Total">Order total amount.</param>
/// <param name="Cancelled">Whether the order was cancelled.</param>
public record OrderSalesSummary(int OrderId, DateTimeOffset OrderDate, decimal Total, bool Cancelled);

/// <summary>
/// Anti-corruption layer that Orders &amp; Replenishment offers to the other bounded contexts.
/// </summary>
public interface IOrdersContextFacade
{
    /// <summary>
    /// Lists the orders of the account placed since a date, including cancelled ones.
    /// </summary>
    Task<IReadOnlyCollection<OrderSalesSummary>> FetchOrdersPlacedSinceAsync(int userId, DateTimeOffset since,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RecentOrderSnapshot>> FetchRecentOrdersAsync(int userId, int limit,
        CancellationToken cancellationToken = default);
}
