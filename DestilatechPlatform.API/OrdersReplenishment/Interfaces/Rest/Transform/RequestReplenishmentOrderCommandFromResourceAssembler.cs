using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Transform;

public static class RequestReplenishmentOrderCommandFromResourceAssembler
{
    public static RequestReplenishmentOrderCommand ToCommandFromResource(CreateReplenishmentOrderResource resource,
        int userId)
    {
        return new RequestReplenishmentOrderCommand(
            userId,
            resource.SupplierName,
            resource.SupplierContact,
            resource.ProductId,
            resource.Quantity,
            resource.ExpectedDate,
            resource.Notes);
    }
}
