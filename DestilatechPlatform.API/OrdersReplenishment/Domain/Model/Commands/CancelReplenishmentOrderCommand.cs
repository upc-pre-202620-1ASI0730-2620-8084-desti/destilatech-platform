namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;

/// <summary>
/// Cancels a replenishment order that has not been received yet.
/// </summary>
public record CancelReplenishmentOrderCommand(int ReplenishmentOrderId, int UserId);
