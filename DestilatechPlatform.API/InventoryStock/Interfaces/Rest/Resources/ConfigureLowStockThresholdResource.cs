namespace DestilatechPlatform.API.InventoryStock.Interfaces.Rest.Resources;

public record ConfigureLowStockThresholdResource(
    int TenantId,
    decimal LowStockThreshold);
