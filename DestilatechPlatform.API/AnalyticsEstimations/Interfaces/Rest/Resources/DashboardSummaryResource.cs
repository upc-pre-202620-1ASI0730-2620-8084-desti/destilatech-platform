using System.Text.Json.Serialization;

namespace DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Resources;

/// <summary>
/// Dashboard summary. The indicators of the other business type are not included in the response.
/// </summary>
public record DashboardSummaryResource(
    string UserType,
    int PendingAlerts,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IEnumerable<DashboardBatchResource>? ActiveBatches,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IEnumerable<DashboardStockLevelResource>? InventoryLevels,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IEnumerable<DashboardStockLevelResource>? LowStockProducts,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IEnumerable<DashboardOrderResource>? RecentOrders);
