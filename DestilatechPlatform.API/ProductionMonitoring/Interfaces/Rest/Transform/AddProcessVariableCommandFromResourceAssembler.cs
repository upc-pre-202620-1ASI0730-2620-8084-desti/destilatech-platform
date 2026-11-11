using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Commands;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Transform;

public static class AddProcessVariableCommandFromResourceAssembler
{
    public static AddProcessVariableCommand ToCommandFromResource(CreateProcessVariableResource resource, int userId)
    {
        return new AddProcessVariableCommand(userId, resource.BatchId, resource.Type, resource.MinRange,
            resource.MaxRange, resource.SensorId);
    }
}
