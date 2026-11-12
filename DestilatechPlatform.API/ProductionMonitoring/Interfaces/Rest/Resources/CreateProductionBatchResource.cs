namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

public record CreateProductionBatchResource(
    int TenantId,
    string Code,
    string Variety,
    DateTimeOffset StartDate,
    decimal EstimatedQuantity,
    string? Tank,
    string? Notes);
