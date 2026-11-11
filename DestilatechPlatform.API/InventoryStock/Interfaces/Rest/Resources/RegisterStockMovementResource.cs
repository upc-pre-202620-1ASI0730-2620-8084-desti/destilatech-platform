namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;

public record RegisterStockMovementResource(
    int ProductId,
    string Type,
    string Reason,
    decimal Quantity,
    string? Notes,
    string? Reference);
