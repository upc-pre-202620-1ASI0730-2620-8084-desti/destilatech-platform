namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Commands;

/// <summary>
/// Adds a monitored process variable, with its allowed range, to a batch.
/// </summary>
public record AddProcessVariableCommand(
    int UserId,
    int BatchId,
    string Type,
    decimal MinRange,
    decimal MaxRange,
    string? SensorId);
