namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;

/// <summary>
/// Dashboard summary of an account. Only the indicators of its business type are filled:
/// a producer gets active batches and inventory levels; a retailer gets low stock products and recent orders.
/// </summary>
public record DashboardSummary(
    string UserType,
    int PendingAlerts,
    IReadOnlyCollection<DashboardBatch>? ActiveBatches,
    IReadOnlyCollection<DashboardStockLevel>? InventoryLevels,
    IReadOnlyCollection<DashboardStockLevel>? LowStockProducts,
    IReadOnlyCollection<DashboardOrder>? RecentOrders);
