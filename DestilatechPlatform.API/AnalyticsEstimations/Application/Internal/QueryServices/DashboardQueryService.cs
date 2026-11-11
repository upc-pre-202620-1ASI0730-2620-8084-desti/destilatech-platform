using DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.OutboundServices;
using DestilatechPlatform.API.AnalyticsEstimations.Application.Services;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Queries;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Shared.Application.Model;

namespace DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.QueryServices;

public class DashboardQueryService(IDashboardSourcesReadPort dashboardSourcesReadPort) : IDashboardQueryService
{
    private const string Producer = "PRODUCER";
    private const int RecentOrdersLimit = 5;

    public async Task<Result<DashboardSummary>> Handle(GetDashboardSummaryQuery query,
        CancellationToken cancellationToken = default)
    {
        var userType = await dashboardSourcesReadPort.ReadUserTypeAsync(query.UserId, cancellationToken);
        if (userType is null)
            return Result<DashboardSummary>.Failure(AnalyticsEstimationsError.UserNotFound(query.UserId));

        var pendingAlerts = await dashboardSourcesReadPort.CountPendingAlertsAsync(query.UserId, cancellationToken);
        var stockLevels = await dashboardSourcesReadPort.ReadStockLevelsAsync(query.UserId, cancellationToken);

        if (userType == Producer)
        {
            var activeBatches = await dashboardSourcesReadPort.ReadActiveBatchesAsync(query.UserId, cancellationToken);
            return Result<DashboardSummary>.Success(
                new DashboardSummary(userType, pendingAlerts, activeBatches, stockLevels, null, null));
        }

        var lowStockProducts = stockLevels.Where(level => level.IsLowStock).ToList();
        var recentOrders = await dashboardSourcesReadPort.ReadRecentOrdersAsync(query.UserId, RecentOrdersLimit,
            cancellationToken);
        return Result<DashboardSummary>.Success(
            new DashboardSummary(userType, pendingAlerts, null, null, lowStockProducts, recentOrders));
    }
}
