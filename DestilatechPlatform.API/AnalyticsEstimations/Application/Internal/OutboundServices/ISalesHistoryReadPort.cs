using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;

namespace DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.OutboundServices;

/// <summary>
/// Read-only outbound port towards Orders &amp; Replenishment, used for the sales volume indicator.
/// </summary>
public interface ISalesHistoryReadPort
{
    /// <summary>
    /// Reads the orders of the account placed since a date, including cancelled ones.
    /// </summary>
    Task<IReadOnlyCollection<SaleRecord>> ReadSalesAsync(int userId, DateTimeOffset since,
        CancellationToken cancellationToken = default);
}
