namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

public record ProcessVariableResource(
    int Id,
    int TenantId,
    int BatchId,
    string Type,
    decimal MinRange,
    decimal MaxRange,
    string? SensorId);
