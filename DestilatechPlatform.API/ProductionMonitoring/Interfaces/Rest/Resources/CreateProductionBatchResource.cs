namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

public record CreateProductionBatchResource(
    string Code,
    string Variety,
    DateTimeOffset StartDate,
    decimal EstimatedQuantity,
    string? Tank,
    string? Notes);
