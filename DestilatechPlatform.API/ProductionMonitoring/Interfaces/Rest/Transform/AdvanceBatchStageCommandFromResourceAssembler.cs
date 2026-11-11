using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Commands;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Transform;

public static class AdvanceBatchStageCommandFromResourceAssembler
{
    public static AdvanceBatchStageCommand ToCommandFromResource(UpdateBatchStatusResource resource, int batchId,
        int userId)
    {
        return new AdvanceBatchStageCommand(userId, batchId, resource.Stage, resource.Notes, resource.ProductId,
            resource.BottledUnits, resource.ConfirmOverEstimate, resource.BottleVolumeLiters);
    }
}
