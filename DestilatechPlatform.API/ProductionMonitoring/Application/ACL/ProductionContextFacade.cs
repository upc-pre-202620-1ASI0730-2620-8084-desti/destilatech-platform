using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Repositories;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.ACL;

namespace DestilatechPlatform.API.ProductionMonitoring.Application.ACL;

public class ProductionContextFacade(IProductionBatchRepository productionBatchRepository) : IProductionContextFacade
{
    private const string BottledStage = "BOTTLED";

    public async Task<IReadOnlyCollection<ProductionBatchSnapshot>> FetchActiveBatchesAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        var batches = await productionBatchRepository.FindByUserIdAsync(userId, cancellationToken);
        return batches
            .Where(batch => batch.Stage != BottledStage)
            .Select(ToSnapshot)
            .ToList();
    }

    public async Task<IReadOnlyCollection<ProductionBatchSnapshot>> FetchBatchesStartedSinceAsync(int userId,
        DateTime sinceUtc, CancellationToken cancellationToken = default)
    {
        var batches = await productionBatchRepository.FindByUserIdAsync(userId, cancellationToken);
        return batches
            .Where(batch => batch.StartDate >= sinceUtc)
            .OrderBy(batch => batch.StartDate)
            .Select(ToSnapshot)
            .ToList();
    }

    private static ProductionBatchSnapshot ToSnapshot(ProductionBatch batch)
    {
        return new ProductionBatchSnapshot(batch.Id, batch.Code, batch.Variety, batch.Stage, batch.EstimatedQuantity,
            batch.StartDate);
    }
}
