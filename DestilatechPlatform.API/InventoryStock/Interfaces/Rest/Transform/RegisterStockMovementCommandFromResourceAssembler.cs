using DestilatechPlatform.API.InventoryStock.Domain.Model.Commands;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Transform;

public static class RegisterStockMovementCommandFromResourceAssembler
{
    public static RegisterStockMovementCommand ToCommandFromResource(RegisterStockMovementResource resource,
        int userId)
    {
        return new RegisterStockMovementCommand(userId, resource.ProductId, resource.Type, resource.Reason,
            resource.Quantity, resource.Notes, resource.Reference);
    }
}
