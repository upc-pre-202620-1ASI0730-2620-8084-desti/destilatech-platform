using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.ValueObjects;

namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;

/// <summary>
/// Replaces the data of a customer that belongs to the account of the user.
/// </summary>
public record UpdateCustomerCommand(
    int CustomerId,
    int UserId,
    string Name,
    string? ContactName,
    string? Phone,
    string? Email,
    string? Address,
    CustomerType CustomerType);
