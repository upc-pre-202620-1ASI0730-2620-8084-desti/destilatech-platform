namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.ACL;

/// <summary>
/// Production batch read by other bounded contexts. StartDate is in UTC.
/// </summary>
public record ProductionBatchSnapshot(
    int BatchId,
    string Code,
    string Variety,
    string Stage,
    decimal EstimatedQuantity,
    DateTime StartDate);
