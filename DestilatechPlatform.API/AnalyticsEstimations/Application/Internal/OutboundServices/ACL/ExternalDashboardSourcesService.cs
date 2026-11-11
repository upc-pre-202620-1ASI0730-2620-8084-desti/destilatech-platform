using DestilatechPlatform.API.AlertsNotifications.Interfaces.ACL;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Iam.Interfaces.Acl;
using DestilatechPlatform.API.InventoryStock.Interfaces.ACL;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.ACL;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.ACL;

namespace DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.OutboundServices.ACL;

/// <summary>
/// Adapter of <see cref="IDashboardSourcesReadPort"/> over the context facades of IAM, Alerts, Production,
/// Inventory and Orders. Analytics only reads; it never changes the other contexts.
/// </summary>
public class ExternalDashboardSourcesService(
    IIamContextFacade iamContextFacade,
    IAlertsContextFacade alertsContextFacade,
    IProductionContextFacade productionContextFacade,
    IInventoryContextFacade inventoryContextFacade,
    IOrdersContextFacade ordersContextFacade) : IDashboardSourcesReadPort
{
    public Task<string?> ReadUserTypeAsync(int userId, CancellationToken cancellationToken = default)
    {
        return iamContextFacade.FetchBusinessTypeAsync(userId, cancellationToken);
    }

    public Task<int> CountPendingAlertsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return alertsContextFacade.CountPendingAlertsAsync(userId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<DashboardBatch>> ReadActiveBatchesAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        var batches = await productionContextFacade.FetchActiveBatchesAsync(userId, cancellationToken);
        return batches
            .Select(batch => new DashboardBatch(batch.BatchId, batch.Code, batch.Variety, batch.Stage,
                batch.EstimatedQuantity))
            .ToList();
    }

    public async Task<IReadOnlyCollection<DashboardStockLevel>> ReadStockLevelsAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        var stock = await inventoryContextFacade.FetchStockOverviewAsync(userId, cancellationToken);
        return stock
            .Select(item => new DashboardStockLevel(item.ProductId, item.Name, item.Presentation, item.CurrentQuantity,
                item.LowStockThreshold, item.IsLowStock))
            .ToList();
    }

    public async Task<IReadOnlyCollection<DashboardOrder>> ReadRecentOrdersAsync(int userId, int limit,
        CancellationToken cancellationToken = default)
    {
        var orders = await ordersContextFacade.FetchRecentOrdersAsync(userId, limit, cancellationToken);
        return orders
            .Select(order => new DashboardOrder(order.OrderId, order.Code, order.CustomerName, order.OrderDate,
                order.Total, order.Status))
            .ToList();
    }
}
