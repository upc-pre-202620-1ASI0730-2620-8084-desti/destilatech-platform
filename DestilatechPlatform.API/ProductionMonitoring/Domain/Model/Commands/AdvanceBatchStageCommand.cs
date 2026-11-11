namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Commands;

/// <summary>
/// Advances a batch to its next stage. Bottling needs the product and the number of bottles.
/// </summary>
public record AdvanceBatchStageCommand(
    int UserId,
    int BatchId,
    string Stage,
    string? Notes,
    int? ProductId,
    int BottledUnits,
    bool ConfirmOverEstimate,
    decimal BottleVolumeLiters);
