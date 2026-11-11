namespace DestilatechPlatform.API.InventoryStock.Interfaces.ACL;

/// <summary>
/// Stock of an active product with its name, used by dashboards.
/// </summary>
public record StockOverviewSnapshot(
    int ProductId,
    string Name,
    string Presentation,
    decimal CurrentQuantity,
    decimal LowStockThreshold,
    bool IsLowStock);
