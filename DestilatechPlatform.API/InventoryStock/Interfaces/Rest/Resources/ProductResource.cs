namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;

public record ProductResource(
    int Id,
    int TenantId,
    string Name,
    string Presentation,
    string Unit,
    string Category,
    decimal UnitPrice,
    string Sku,
    bool IsActive);
