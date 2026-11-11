namespace DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Resources;

public record DashboardStockLevelResource(
    int ProductId,
    string Name,
    string Presentation,
    decimal CurrentQuantity,
    decimal LowStockThreshold,
    bool IsLowStock);
