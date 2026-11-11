namespace DestilatechPlatform.API.InventoryStock.Domain.Model.Commands;

/// <summary>
/// Updates the data of a product of the account.
/// </summary>
public record UpdateProductCommand(
    int UserId,
    int ProductId,
    string Name,
    string? Presentation,
    string Unit,
    string? Category,
    decimal UnitPrice,
    string? Sku);
