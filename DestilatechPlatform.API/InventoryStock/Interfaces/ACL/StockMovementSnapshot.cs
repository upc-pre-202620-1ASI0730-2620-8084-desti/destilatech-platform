namespace DestilatechPlatform.API.InventoryStock.Interfaces.ACL;

/// <summary>
/// Stock movement read by other bounded contexts. MovementDate is in UTC.
/// </summary>
public record StockMovementSnapshot(int ProductId, string Type, string Reason, decimal Quantity, DateTime MovementDate);
