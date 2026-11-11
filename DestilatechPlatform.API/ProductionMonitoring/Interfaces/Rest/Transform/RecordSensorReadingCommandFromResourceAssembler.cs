using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Commands;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Transform;

public static class RecordSensorReadingCommandFromResourceAssembler
{
    public static RecordSensorReadingCommand ToCommandFromResource(RecordSensorReadingResource resource, int userId)
    {
        return new RecordSensorReadingCommand(userId, resource.ProcessVariableId, null, null, resource.SourceKey,
            resource.Value, resource.RecordedAt);
    }

    public static RecordSensorReadingCommand ToCommandFromResource(IngestBatchReadingResource resource, int batchId,
        int userId)
    {
        return new RecordSensorReadingCommand(userId, null, batchId, resource.SourceId.Trim(), resource.SourceKey,
            resource.Value, resource.RecordedAt);
    }
}
