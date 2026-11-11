using DestilatechPlatform.API.InventoryStock.Domain.Model.Commands;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Transform;

public static class UpdateProductCommandFromResourceAssembler
{
    public static UpdateProductCommand ToCommandFromResource(UpdateProductResource resource, int productId,
        int userId)
    {
        return new UpdateProductCommand(userId, productId, resource.Name, resource.Presentation, resource.Unit,
            resource.Category, resource.UnitPrice, resource.Sku);
    }
}
