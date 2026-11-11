using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;

namespace DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.OutboundServices;

/// <summary>
/// Outbound port that reads, from the other bounded contexts, the data shown in the dashboards.
/// </summary>
public interface IDashboardSourcesReadPort
{
    /// <returns>PRODUCER, RETAILER, or <c>null</c> if the account does not exist.</returns>
    Task<string?> ReadUserTypeAsync(int userId, CancellationToken cancellationToken = default);

    Task<int> CountPendingAlertsAsync(int userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DashboardBatch>> ReadActiveBatchesAsync(int userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DashboardStockLevel>> ReadStockLevelsAsync(int userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DashboardOrder>> ReadRecentOrdersAsync(int userId, int limit,
        CancellationToken cancellationToken = default);
}
