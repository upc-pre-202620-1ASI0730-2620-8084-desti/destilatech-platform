using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.ValueObjects;

namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;

/// <summary>
/// Registers a customer for the account of the user (US17).
/// </summary>
/// <param name="UserId">Account that owns the customer.</param>
/// <param name="Name">Business or person name. Required.</param>
/// <param name="ContactName">Person to talk to. Optional.</param>
/// <param name="Phone">Phone number. A phone or an email is required.</param>
/// <param name="Email">Email address. A phone or an email is required.</param>
/// <param name="Address">Delivery address. Optional.</param>
/// <param name="CustomerType">Kind of customer.</param>
public record RegisterCustomerCommand(
    int UserId,
    string Name,
    string? ContactName,
    string? Phone,
    string? Email,
    string? Address,
    CustomerType CustomerType);
