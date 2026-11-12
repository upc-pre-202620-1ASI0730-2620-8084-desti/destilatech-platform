using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Shared.Domain.Model.Aggregates.Internal;

namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;

/// <summary>
/// Customer of a producer or trader (US17). A customer needs a name and at least
/// a phone number or an email to be contacted.
/// </summary>
public partial class Customer : AggregateRoot
{
    public int Id { get; private set; }

    public int UserId { get; private set; }

    public string Name { get; private set; }

    public string ContactName { get; private set; }

    public string Phone { get; private set; }

    public string Email { get; private set; }

    public string Address { get; private set; }

    public CustomerType CustomerType { get; private set; }

    /// <summary>
    /// Constructor required by Entity Framework Core.
    /// </summary>
    protected Customer()
    {
        Name = string.Empty;
        ContactName = string.Empty;
        Phone = string.Empty;
        Email = string.Empty;
        Address = string.Empty;
    }

    public Customer(RegisterCustomerCommand command) : this()
    {
        if (command.UserId <= 0)
        {
            throw new OrdersReplenishmentException(
                OrdersReplenishmentError.InvalidData("A customer must belong to an account."));
        }

        UserId = command.UserId;
        ApplyDetails(command.Name, command.ContactName, command.Phone, command.Email, command.Address,
            command.CustomerType);
    }

    public void Update(UpdateCustomerCommand command)
    {
        ApplyDetails(command.Name, command.ContactName, command.Phone, command.Email, command.Address,
            command.CustomerType);
    }

    public bool BelongsTo(int userId)
    {
        return UserId == userId;
    }

    private void ApplyDetails(string name, string? contactName, string? phone, string? email, string? address,
        CustomerType customerType)
    {
        var trimmedName = name?.Trim() ?? string.Empty;
        var trimmedPhone = phone?.Trim() ?? string.Empty;
        var trimmedEmail = email?.Trim() ?? string.Empty;

        if (trimmedName.Length == 0)
        {
            throw new OrdersReplenishmentException(OrdersReplenishmentError.NameRequired());
        }

        if (trimmedPhone.Length == 0 && trimmedEmail.Length == 0)
        {
            throw new OrdersReplenishmentException(OrdersReplenishmentError.ContactRequired());
        }

        if (!Enum.IsDefined(customerType))
        {
            throw new OrdersReplenishmentException(
                OrdersReplenishmentError.InvalidData($"Unsupported customer type: {customerType}."));
        }

        Name = trimmedName;
        ContactName = contactName?.Trim() ?? string.Empty;
        Phone = trimmedPhone;
        Email = trimmedEmail;
        Address = address?.Trim() ?? string.Empty;
        CustomerType = customerType;
    }
}
