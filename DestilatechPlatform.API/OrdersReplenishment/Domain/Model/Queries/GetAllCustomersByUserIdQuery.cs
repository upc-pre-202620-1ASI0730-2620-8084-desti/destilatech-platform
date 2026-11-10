namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;

/// <summary>
/// Lists the customers of the account, sorted by name.
/// </summary>
public record GetAllCustomersByUserIdQuery(int UserId);
