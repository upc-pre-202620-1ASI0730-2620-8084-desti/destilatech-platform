namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;

public record CreateProductResource(
    string Name,
    string? Presentation,
    string Unit,
    string? Category,
    decimal UnitPrice,
    string? Sku,
    decimal LowStockThreshold = 0);
