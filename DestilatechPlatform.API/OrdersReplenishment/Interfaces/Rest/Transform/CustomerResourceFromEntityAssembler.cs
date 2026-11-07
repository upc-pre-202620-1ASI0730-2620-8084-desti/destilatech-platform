using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Transform;

public static class CustomerResourceFromEntityAssembler
{
    public static CustomerResource ToResourceFromEntity(Customer entity)
    {
        return new CustomerResource(
            entity.Id,
            entity.UserId,
            entity.Name,
            entity.ContactName,
            entity.Phone,
            entity.Email,
            entity.Address,
            OrdersReplenishmentValueAssembler.ToValueFromEnum(entity.CustomerType));
    }
}
