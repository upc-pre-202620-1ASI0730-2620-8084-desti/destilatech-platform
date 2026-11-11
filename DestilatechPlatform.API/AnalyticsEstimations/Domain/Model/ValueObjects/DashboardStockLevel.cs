namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;

/// <summary>
/// Stock level of a product shown in a dashboard.
/// </summary>
public record DashboardStockLevel(
    int ProductId,
    string Name,
    string Presentation,
    decimal CurrentQuantity,
    decimal LowStockThreshold,
    bool IsLowStock);
