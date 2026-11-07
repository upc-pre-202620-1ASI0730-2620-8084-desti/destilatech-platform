using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Transform;

public static class OrderResourceFromEntityAssembler
{
    public static OrderResource ToResourceFromEntity(Order entity)
    {
        var lines = entity.Lines
            .Select(line => new OrderLineResource(line.ProductId, line.ProductName, line.Quantity, line.UnitPrice,
                line.Subtotal()))
            .ToList();
        var nextStatus = entity.NextStatus();
        return new OrderResource(
            entity.Id,
            entity.UserId,
            entity.Code,
            entity.CustomerId,
            entity.CustomerName,
            entity.OrderDate,
            OrdersReplenishmentValueAssembler.ToValueFromEnum(entity.Status),
            nextStatus is null ? null : OrdersReplenishmentValueAssembler.ToValueFromEnum(nextStatus.Value),
            lines,
            entity.Total(),
            entity.TotalUnits(),
            entity.Notes);
    }
}
