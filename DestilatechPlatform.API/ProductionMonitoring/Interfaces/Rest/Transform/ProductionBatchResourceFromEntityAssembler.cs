using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Transform;

public static class ProductionBatchResourceFromEntityAssembler
{
    public static ProductionBatchResource ToResourceFromEntity(ProductionBatch entity)
    {
        var stageHistory = entity.StageHistory
            .OrderBy(change => change.ChangedAt)
            .ThenBy(change => change.Id)
            .Select(change => new BatchStageChangeResource(change.Id, change.Stage, change.ChangedAt, change.Notes));
        return new ProductionBatchResource(entity.Id, entity.UserId, entity.Code, entity.Variety, entity.StartDate,
            entity.EstimatedQuantity, entity.Tank, entity.Stage, entity.Notes, entity.ProductId, entity.BottledUnits,
            stageHistory.ToList());
    }
}
