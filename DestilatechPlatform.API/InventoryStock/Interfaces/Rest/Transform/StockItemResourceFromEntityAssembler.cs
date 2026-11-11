using DestilatechPlatform.API.InventoryStock.Domain.Model.Aggregates;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Transform;

public static class StockItemResourceFromEntityAssembler
{
    public static StockItemResource ToResourceFromEntity(StockItem entity)
    {
        return new StockItemResource(entity.Id, entity.ProductId, entity.UserId, entity.CurrentQuantity,
            entity.LowStockThreshold, entity.Version);
    }
}
