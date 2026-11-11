namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Commands;

/// <summary>
/// Updates the details of a batch and, when the stage changes, advances it to the next stage.
/// </summary>
public record UpdateProductionBatchCommand(
    int UserId,
    int BatchId,
    string Stage,
    string Variety,
    decimal EstimatedQuantity,
    string? Tank,
    string? Notes,
    int? ProductId,
    int BottledUnits,
    string? StageNotes,
    bool ConfirmOverEstimate,
    decimal BottleVolumeLiters);
