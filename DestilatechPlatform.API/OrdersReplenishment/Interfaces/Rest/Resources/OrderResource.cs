namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;

/// <summary>
/// Order as returned by the API, with the same fields the web app uses.
/// </summary>
/// <param name="NextStatus">Status the order can advance to, or <c>null</c> when it is delivered or cancelled.</param>
public record OrderResource(
    int Id,
    int UserId,
    string Code,
    int CustomerId,
    string CustomerName,
    DateTimeOffset OrderDate,
    string Status,
    string? NextStatus,
    IReadOnlyCollection<OrderLineResource> Lines,
    decimal Total,
    int TotalUnits,
    string Notes);
