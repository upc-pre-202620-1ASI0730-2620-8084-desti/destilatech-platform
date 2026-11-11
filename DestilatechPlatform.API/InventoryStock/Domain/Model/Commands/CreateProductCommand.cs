namespace DestilatechPlatform.API.InventoryStock.Domain.Model.Commands;

/// <summary>
/// Registers a product of the account together with its stock item.
/// </summary>
public record CreateProductCommand(
    int UserId,
    string Name,
    string? Presentation,
    string Unit,
    string? Category,
    decimal UnitPrice,
    string? Sku,
    decimal LowStockThreshold);
