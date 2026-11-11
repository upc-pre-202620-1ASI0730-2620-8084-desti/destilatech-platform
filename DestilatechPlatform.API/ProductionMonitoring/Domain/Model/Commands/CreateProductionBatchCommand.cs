namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Commands;

/// <summary>
/// Registers a production batch of the account in the RECEIVED stage.
/// </summary>
public record CreateProductionBatchCommand(
    int UserId,
    string Code,
    string Variety,
    DateTimeOffset StartDate,
    decimal EstimatedQuantity,
    string? Tank,
    string? Notes);
