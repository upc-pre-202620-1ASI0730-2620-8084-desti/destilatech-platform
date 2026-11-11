namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;

/// <summary>
/// Gets a customer, only if it belongs to the account of the user.
/// </summary>
public record GetCustomerByIdQuery(int CustomerId, int UserId);
