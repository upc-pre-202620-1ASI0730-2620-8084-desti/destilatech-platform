namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;

/// <summary>
/// Lists the orders placed by one customer of the account, newest first.
/// </summary>
public record GetOrdersByCustomerIdQuery(int CustomerId, int UserId);
