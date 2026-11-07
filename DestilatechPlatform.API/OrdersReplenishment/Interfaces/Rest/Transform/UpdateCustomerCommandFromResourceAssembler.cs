using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.ValueObjects;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Transform;

public static class UpdateCustomerCommandFromResourceAssembler
{
    /// <exception cref="ArgumentException">The customer type is not valid.</exception>
    public static UpdateCustomerCommand ToCommandFromResource(int customerId, UpdateCustomerResource resource,
        int userId)
    {
        return new UpdateCustomerCommand(
            customerId,
            userId,
            resource.Name,
            resource.ContactName,
            resource.Phone,
            resource.Email,
            resource.Address,
            OrdersReplenishmentValueAssembler.ToEnumFromValue<CustomerType>(resource.CustomerType));
    }
}
