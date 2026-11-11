namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

public record ProcessVariableResource(
    int Id,
    int UserId,
    int BatchId,
    string Type,
    decimal MinRange,
    decimal MaxRange,
    string? SensorId);
