using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.ACL;

namespace DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.OutboundServices.ACL;

/// <summary>
/// Adapter of <see cref="IProductionHistoryReadPort"/> over the Production &amp; Monitoring bounded context.
/// The production of a period is the estimated liters of the batches started in it.
/// </summary>
public class ExternalProductionHistoryService(IProductionContextFacade productionContextFacade)
    : IProductionHistoryReadPort
{
    public async Task<IReadOnlyCollection<ProductionRecord>> ReadProductionAsync(int userId, DateTimeOffset since,
        CancellationToken cancellationToken = default)
    {
        var batches = await productionContextFacade.FetchBatchesStartedSinceAsync(userId, since.UtcDateTime,
            cancellationToken);
        return batches
            .Select(batch => new ProductionRecord(
                new DateTimeOffset(DateTime.SpecifyKind(batch.StartDate, DateTimeKind.Utc)), batch.EstimatedQuantity))
            .ToList();
    }
}
