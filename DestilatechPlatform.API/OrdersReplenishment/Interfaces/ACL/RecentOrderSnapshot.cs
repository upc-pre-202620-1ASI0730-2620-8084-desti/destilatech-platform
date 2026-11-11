namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.ACL;

/// <summary>
/// Recent order of an account, used by dashboards. Status uses the API values (PENDING, PREPARING…).
/// </summary>
public record RecentOrderSnapshot(
    int OrderId,
    string Code,
    string CustomerName,
    DateTimeOffset OrderDate,
    decimal Total,
    string Status);
