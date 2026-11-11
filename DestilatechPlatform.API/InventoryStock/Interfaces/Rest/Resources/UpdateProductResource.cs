namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;

public record UpdateProductResource(
    string Name,
    string? Presentation,
    string Unit,
    string? Category,
    decimal UnitPrice,
    string? Sku);
