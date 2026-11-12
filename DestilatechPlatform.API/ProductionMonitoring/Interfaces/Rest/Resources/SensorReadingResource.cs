namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

public record SensorReadingResource(
    int Id,
    int ProcessVariableId,
    int BatchId,
    int TenantId,
    decimal Value,
    string? SourceKey,
    bool WithinRange,
    DateTime RecordedAt);
