namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;

/// <summary>
/// Gets a replenishment order, only if it belongs to the account of the user.
/// </summary>
public record GetReplenishmentOrderByIdQuery(int ReplenishmentOrderId, int UserId);
