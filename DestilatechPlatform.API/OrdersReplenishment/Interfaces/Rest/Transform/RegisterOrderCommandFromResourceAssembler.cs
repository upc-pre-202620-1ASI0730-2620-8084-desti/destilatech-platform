using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Transform;

public static class RegisterOrderCommandFromResourceAssembler
{
    public static RegisterOrderCommand ToCommandFromResource(CreateOrderResource resource, int userId)
    {
        var lines = (resource.Lines ?? [])
            .Select(line => new OrderLineData(line.ProductId, line.Quantity))
            .ToList();
        return new RegisterOrderCommand(userId, resource.CustomerId, lines, resource.Notes);
    }
}
