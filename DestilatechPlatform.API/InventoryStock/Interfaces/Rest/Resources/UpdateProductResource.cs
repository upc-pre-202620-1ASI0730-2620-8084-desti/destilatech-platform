namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;

public record UpdateProductResource(
    int TenantId,
    string Name,
    string? Presentation,
    string Unit,
    string? Category,
    decimal UnitPrice,
    string? Sku);
