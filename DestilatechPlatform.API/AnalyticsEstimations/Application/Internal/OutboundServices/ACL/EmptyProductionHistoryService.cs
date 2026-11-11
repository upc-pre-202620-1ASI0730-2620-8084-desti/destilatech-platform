using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;

namespace DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.OutboundServices.ACL;

/// <summary>
/// Temporary implementation of <see cref="IProductionHistoryReadPort"/> that returns no batches.
/// </summary>
/// <remarks>
/// The Production bounded context is not in the platform yet, so the production volume indicator
/// stays at zero. Replace this class with an adapter over its facade when it is added.
/// </remarks>
public class EmptyProductionHistoryService : IProductionHistoryReadPort
{
    public Task<IReadOnlyCollection<ProductionRecord>> ReadProductionAsync(int userId, DateTimeOffset since,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyCollection<ProductionRecord>>([]);
    }
}
