namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;

/// <summary>
/// Data to register an order. Other fields the web app may send (code, prices, total) are ignored:
/// the API generates the code and takes names and prices from the inventory.
/// </summary>
public record CreateOrderResource(
    int TenantId,
    int CustomerId,
    IReadOnlyCollection<CreateOrderLineResource>? Lines,
    string? Notes);
