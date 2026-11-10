namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;

/// <summary>
/// Cancels an open order and returns its products to stock.
/// </summary>
public record CancelOrderCommand(int OrderId, int UserId);
