using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Transform;

public static class ProcessVariableResourceFromEntityAssembler
{
    public static ProcessVariableResource ToResourceFromEntity(ProcessVariable entity)
    {
        return new ProcessVariableResource(entity.Id, entity.UserId, entity.BatchId, entity.Type, entity.MinRange,
            entity.MaxRange, entity.SensorId);
    }
}
