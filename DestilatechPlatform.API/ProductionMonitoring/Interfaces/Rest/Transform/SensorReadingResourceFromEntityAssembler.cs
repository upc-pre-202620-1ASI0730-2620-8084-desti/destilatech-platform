using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Entities;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Transform;

public static class SensorReadingResourceFromEntityAssembler
{
    public static SensorReadingResource ToResourceFromEntity(SensorReading entity)
    {
        return new SensorReadingResource(entity.Id, entity.ProcessVariableId, entity.BatchId, entity.UserId,
            entity.Value, entity.SourceKey, entity.WithinRange, entity.RecordedAt);
    }
}
