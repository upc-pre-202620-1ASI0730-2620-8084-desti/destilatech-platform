namespace DestilatechPlatform.API.InventoryStock.Domain.Model.Commands;

/// <summary>
/// Registers a manual stock entry (IN) or exit (OUT) of a product.
/// </summary>
public record RegisterStockMovementCommand(
    int UserId,
    int ProductId,
    string Type,
    string Reason,
    decimal Quantity,
    string? Notes,
    string? Reference);
