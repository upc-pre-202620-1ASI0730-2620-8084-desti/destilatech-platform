using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;

namespace DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.OutboundServices;

/// <summary>
/// Read-only outbound port towards Production &amp; Monitoring, used for the production volume indicator.
/// </summary>
public interface IProductionHistoryReadPort
{
    /// <summary>
    /// Reads the batches started since a date. Accounts that are not producers have no batches.
    /// </summary>
    Task<IReadOnlyCollection<ProductionRecord>> ReadProductionAsync(int userId, DateTimeOffset since,
        CancellationToken cancellationToken = default);
}
