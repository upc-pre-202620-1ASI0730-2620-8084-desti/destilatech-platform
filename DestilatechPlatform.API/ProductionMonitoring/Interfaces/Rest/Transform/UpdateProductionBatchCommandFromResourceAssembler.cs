using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Commands;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Transform;

public static class UpdateProductionBatchCommandFromResourceAssembler
{
    public static UpdateProductionBatchCommand ToCommandFromResource(UpdateProductionBatchResource resource,
        int batchId, int userId)
    {
        return new UpdateProductionBatchCommand(userId, batchId, resource.Stage, resource.Variety,
            resource.EstimatedQuantity, resource.Tank, resource.Notes, resource.ProductId, resource.BottledUnits,
            resource.StageNotes, resource.ConfirmOverEstimate, resource.BottleVolumeLiters);
    }
}
