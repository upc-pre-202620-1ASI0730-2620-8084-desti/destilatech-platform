using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Entities;
using DestilatechPlatform.API.Shared.Domain.Repositories;

namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Repositories;

public interface ISensorReadingRepository : IBaseRepository<SensorReading>
{
    Task<IEnumerable<SensorReading>> FindLatestByUserIdAsync(int userId, int limit,
        CancellationToken cancellationToken = default);

    Task<SensorReading?> FindBySourceKeyAsync(int userId, int processVariableId, string sourceKey,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<SensorReading>> FindByBatchIdAsync(int userId, int batchId, string? variableType,
        DateTime? fromUtc, DateTime? toUtc, CancellationToken cancellationToken = default);
}
