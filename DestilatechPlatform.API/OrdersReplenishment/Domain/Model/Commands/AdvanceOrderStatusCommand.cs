namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;

/// <summary>
/// Moves an open order to its next status: Pending, Preparing, Shipped, Delivered.
/// </summary>
public record AdvanceOrderStatusCommand(int OrderId, int UserId);
