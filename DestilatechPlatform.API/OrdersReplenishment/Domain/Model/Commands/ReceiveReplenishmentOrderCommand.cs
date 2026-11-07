namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;

/// <summary>
/// Marks a requested replenishment order as received and adds its units to stock.
/// </summary>
public record ReceiveReplenishmentOrderCommand(int ReplenishmentOrderId, int UserId);
