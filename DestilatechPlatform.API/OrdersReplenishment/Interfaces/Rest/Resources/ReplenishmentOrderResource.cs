namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;

/// <summary>
/// Replenishment order as returned by the API, with the same fields the web app uses.
/// </summary>
public record ReplenishmentOrderResource(
    int Id,
    int UserId,
    string Code,
    string SupplierName,
    string SupplierContact,
    int ProductId,
    string ProductName,
    int Quantity,
    DateTimeOffset OrderDate,
    DateTimeOffset? ExpectedDate,
    DateTimeOffset? ReceivedDate,
    string Status,
    string Notes);
