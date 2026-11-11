using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;
using DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Transform;

public static class DashboardSummaryResourceFromValueAssembler
{
    public static DashboardSummaryResource ToResourceFromValue(DashboardSummary summary)
    {
        return new DashboardSummaryResource(
            summary.UserType,
            summary.PendingAlerts,
            summary.ActiveBatches?.Select(batch => new DashboardBatchResource(batch.BatchId, batch.Code, batch.Variety,
                batch.Stage, batch.EstimatedQuantity)).ToList(),
            summary.InventoryLevels?.Select(ToStockLevelResource).ToList(),
            summary.LowStockProducts?.Select(ToStockLevelResource).ToList(),
            summary.RecentOrders?.Select(order => new DashboardOrderResource(order.OrderId, order.Code,
                order.CustomerName, order.OrderDate, order.Total, order.Status)).ToList());
    }

    private static DashboardStockLevelResource ToStockLevelResource(DashboardStockLevel level)
    {
        return new DashboardStockLevelResource(level.ProductId, level.Name, level.Presentation, level.CurrentQuantity,
            level.LowStockThreshold, level.IsLowStock);
    }
}
