namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Queries;

/// <summary>
/// Lists the readings of a batch, optionally for one variable type and a period.
/// </summary>
public record GetBatchSensorReadingsQuery(
    int UserId,
    int BatchId,
    string? VariableType,
    DateTimeOffset? From,
    DateTimeOffset? To);
