namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

public record CreateProcessVariableResource(
    int BatchId,
    string Type,
    decimal MinRange,
    decimal MaxRange,
    string? SensorId);
