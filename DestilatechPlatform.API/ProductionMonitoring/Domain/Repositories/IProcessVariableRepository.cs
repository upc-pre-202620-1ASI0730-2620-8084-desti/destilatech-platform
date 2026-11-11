using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;
using DestilatechPlatform.API.Shared.Domain.Repositories;

namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Repositories;

public interface IProcessVariableRepository : IBaseRepository<ProcessVariable>
{
    Task<IEnumerable<ProcessVariable>> FindByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<ProcessVariable?> FindByIdAndUserIdAsync(int processVariableId, int userId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByBatchIdAndTypeAsync(int batchId, string type, int userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds the variable that receives a reading, by its id or by its batch and sensor.
    /// </summary>
    Task<ProcessVariable?> FindForReadingAsync(int userId, int? processVariableId, int? batchId, string? sensorId,
        CancellationToken cancellationToken = default);
}
