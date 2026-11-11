namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Commands;

/// <summary>
/// Records a sensor reading, found by process variable or by batch and sensor.
/// </summary>
/// <remarks>
/// A reading with a SourceKey that was already recorded is returned again instead of being duplicated.
/// </remarks>
public record RecordSensorReadingCommand(
    int UserId,
    int? ProcessVariableId,
    int? BatchId,
    string? SourceId,
    string? SourceKey,
    decimal Value,
    DateTimeOffset RecordedAt);
