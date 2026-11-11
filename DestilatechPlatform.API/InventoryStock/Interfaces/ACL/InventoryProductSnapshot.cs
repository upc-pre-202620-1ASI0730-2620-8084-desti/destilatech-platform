namespace DestilatechPlatform.API.InventoryStock.Interfaces.ACL;

/// <summary>
/// Active product of an account, with its current available quantity.
/// </summary>
public record InventoryProductSnapshot(
    int ProductId,
    string Name,
    string Presentation,
    decimal UnitPrice,
    decimal AvailableQuantity);
