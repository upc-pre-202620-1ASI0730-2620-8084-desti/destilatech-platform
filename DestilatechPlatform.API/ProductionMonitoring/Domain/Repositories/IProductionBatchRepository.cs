using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;
using DestilatechPlatform.API.Shared.Domain.Repositories;

namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Repositories;

public interface IProductionBatchRepository : IBaseRepository<ProductionBatch>
{
    Task<IEnumerable<ProductionBatch>> FindByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<ProductionBatch?> FindByIdAndUserIdAsync(int batchId, int userId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByIdAndUserIdAsync(int batchId, int userId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAndUserIdAsync(string code, int userId, CancellationToken cancellationToken = default);
}
