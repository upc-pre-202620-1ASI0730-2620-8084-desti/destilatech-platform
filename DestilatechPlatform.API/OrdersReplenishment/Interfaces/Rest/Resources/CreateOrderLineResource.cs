namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;

/// <summary>
/// Product and quantity of a new order. Name and price are taken from the inventory.
/// </summary>
public record CreateOrderLineResource(int ProductId, int Quantity);
