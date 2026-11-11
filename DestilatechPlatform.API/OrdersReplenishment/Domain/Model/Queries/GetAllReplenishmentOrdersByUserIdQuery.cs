namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;

/// <summary>
/// Lists the replenishment orders of the account, newest first.
/// </summary>
public record GetAllReplenishmentOrdersByUserIdQuery(int UserId);
