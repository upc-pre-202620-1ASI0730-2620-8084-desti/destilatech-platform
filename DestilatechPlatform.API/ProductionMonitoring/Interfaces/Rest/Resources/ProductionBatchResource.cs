namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

public record ProductionBatchResource(
    int Id,
    int TenantId,
    string Code,
    string Variety,
    DateTime StartDate,
    decimal EstimatedQuantity,
    string Tank,
    string Stage,
    string Notes,
    int? ProductId,
    int BottledUnits,
    IEnumerable<BatchStageChangeResource> StageHistory);
