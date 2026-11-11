namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

public record UpdateProductionBatchResource(
    string Stage,
    string Variety,
    decimal EstimatedQuantity,
    string? Tank,
    string? Notes,
    int? ProductId,
    int BottledUnits,
    string? StageNotes,
    bool ConfirmOverEstimate = false,
    decimal BottleVolumeLiters = 0.75m);
