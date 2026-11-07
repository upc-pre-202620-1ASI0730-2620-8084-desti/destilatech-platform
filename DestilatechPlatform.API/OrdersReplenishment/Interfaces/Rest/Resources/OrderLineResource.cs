namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;

/// <summary>
/// Line of an order as returned by the API.
/// </summary>
public record OrderLineResource(int ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal Subtotal);
