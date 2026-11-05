namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;

/// <summary>
/// Product and quantity requested in an order. The product name and unit price
/// are taken from Inventory &amp; Stock Management, not from the client.
/// </summary>
/// <param name="ProductId">Product identifier from Inventory &amp; Stock Management.</param>
/// <param name="Quantity">Units requested. Must be greater than zero.</param>
public record OrderLineData(int ProductId, int Quantity);
