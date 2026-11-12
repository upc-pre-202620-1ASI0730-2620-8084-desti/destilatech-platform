namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;

public record StockItemResource(
    int Id,
    int ProductId,
    int TenantId,
    decimal CurrentQuantity,
    decimal LowStockThreshold,
    int Version);
