namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;

/// <summary>
/// Lists the order history of the account, newest first (US19).
/// </summary>
public record GetAllOrdersByUserIdQuery(int UserId);
