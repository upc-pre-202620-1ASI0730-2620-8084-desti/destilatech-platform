using DestilatechPlatform.API.InventoryStock.Domain.Model.Commands;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Transform;

public static class CreateProductCommandFromResourceAssembler
{
    public static CreateProductCommand ToCommandFromResource(CreateProductResource resource, int userId)
    {
        return new CreateProductCommand(userId, resource.Name, resource.Presentation, resource.Unit,
            resource.Category, resource.UnitPrice, resource.Sku, resource.LowStockThreshold);
    }
}
