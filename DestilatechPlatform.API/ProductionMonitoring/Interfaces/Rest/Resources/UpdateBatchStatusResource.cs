namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;

public record UpdateBatchStatusResource(
    int TenantId,
    string Stage,
    string? Notes,
    int? ProductId,
    int BottledUnits,
    bool ConfirmOverEstimate = false,
    decimal BottleVolumeLiters = 0.75m);
