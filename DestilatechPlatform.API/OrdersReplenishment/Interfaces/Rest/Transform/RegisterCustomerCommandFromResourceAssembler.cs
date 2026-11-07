using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.ValueObjects;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Transform;

public static class RegisterCustomerCommandFromResourceAssembler
{
    /// <exception cref="ArgumentException">The customer type is not valid.</exception>
    public static RegisterCustomerCommand ToCommandFromResource(CreateCustomerResource resource, int userId)
    {
        return new RegisterCustomerCommand(
            userId,
            resource.Name,
            resource.ContactName,
            resource.Phone,
            resource.Email,
            resource.Address,
            OrdersReplenishmentValueAssembler.ToEnumFromValue<CustomerType>(resource.CustomerType));
    }
}
