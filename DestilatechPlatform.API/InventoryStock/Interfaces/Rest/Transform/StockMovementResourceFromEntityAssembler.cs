using DestilatechPlatform.API.InventoryStock.Domain.Model.Entities;
using DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Transform;

public static class StockMovementResourceFromEntityAssembler
{
    public static StockMovementResource ToResourceFromEntity(StockMovement entity)
    {
        return new StockMovementResource(entity.Id, entity.StockItemId, entity.ProductId, entity.UserId, entity.Type,
            entity.Reason, entity.Quantity, entity.Notes, entity.Reference, entity.MovementDate);
    }
}
