using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Transform;

public static class ReplenishmentOrderResourceFromEntityAssembler
{
    public static ReplenishmentOrderResource ToResourceFromEntity(ReplenishmentOrder entity)
    {
        return new ReplenishmentOrderResource(
            entity.Id,
            entity.UserId,
            entity.Code,
            entity.SupplierName,
            entity.SupplierContact,
            entity.ProductId,
            entity.ProductName,
            entity.Quantity,
            entity.OrderDate,
            entity.ExpectedDate,
            entity.ReceivedDate,
            OrdersReplenishmentValueAssembler.ToValueFromEnum(entity.Status),
            entity.Notes);
    }
}
