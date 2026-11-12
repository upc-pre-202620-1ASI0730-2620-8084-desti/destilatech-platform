namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

public record RecordSensorReadingResource(
    int TenantId,
    int ProcessVariableId,
    decimal Value,
    DateTimeOffset RecordedAt,
    string? SourceKey = null);
