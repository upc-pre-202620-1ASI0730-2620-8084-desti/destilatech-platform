namespace DestilatechPlatform.API.OrdersReplenishment.Application.Internal.OutboundServices;

/// <summary>
/// Product data that Orders &amp; Replenishment needs from Inventory &amp; Stock Management.
/// </summary>
/// <param name="ProductId">Product identifier.</param>
/// <param name="DisplayName">Name shown to the user, for example <c>Pisco Italia · 750 ml</c>.</param>
/// <param name="UnitPrice">Current sale price of one unit.</param>
/// <param name="AvailableQuantity">Units currently in stock.</param>
public record InventoryProduct(int ProductId, string DisplayName, decimal UnitPrice, decimal AvailableQuantity);
