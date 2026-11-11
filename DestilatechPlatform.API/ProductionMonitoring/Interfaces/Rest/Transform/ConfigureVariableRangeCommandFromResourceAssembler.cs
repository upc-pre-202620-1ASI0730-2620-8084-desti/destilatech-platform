using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Commands;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Transform;

public static class ConfigureVariableRangeCommandFromResourceAssembler
{
    public static ConfigureVariableRangeCommand ToCommandFromResource(ConfigureVariableRangeResource resource,
        int processVariableId, int userId)
    {
        return new ConfigureVariableRangeCommand(userId, processVariableId, resource.MinRange, resource.MaxRange);
    }
}
