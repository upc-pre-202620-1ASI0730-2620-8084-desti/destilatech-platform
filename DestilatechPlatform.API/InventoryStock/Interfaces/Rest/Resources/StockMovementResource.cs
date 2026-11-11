namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;

public record StockMovementResource(
    int Id,
    int StockItemId,
    int ProductId,
    int UserId,
    string Type,
    string Reason,
    decimal Quantity,
    string Notes,
    string Reference,
    DateTime MovementDate);
