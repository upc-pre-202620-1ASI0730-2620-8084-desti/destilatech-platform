namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;

/// <summary>
/// Gets an order with its lines, only if it belongs to the account of the user.
/// </summary>
public record GetOrderByIdQuery(int OrderId, int UserId);
