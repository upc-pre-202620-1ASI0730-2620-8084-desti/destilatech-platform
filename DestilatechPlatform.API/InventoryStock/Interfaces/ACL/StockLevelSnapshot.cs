namespace DestilatechPlatform.API.InventoryStock.Interfaces.ACL;

/// <summary>
/// Current quantity and low stock threshold of an active product.
/// </summary>
public record StockLevelSnapshot(int ProductId, decimal CurrentQuantity, decimal LowStockThreshold);
