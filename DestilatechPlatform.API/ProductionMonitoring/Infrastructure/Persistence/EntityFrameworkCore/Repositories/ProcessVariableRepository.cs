using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Repositories;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.ProductionMonitoring.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class ProcessVariableRepository(AppDbContext context)
    : BaseRepository<ProcessVariable>(context), IProcessVariableRepository
{
    public async Task<IEnumerable<ProcessVariable>> FindByUserIdAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<ProcessVariable>()
            .AsNoTracking()
            .Where(variable => variable.UserId == userId)
            .OrderBy(variable => variable.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProcessVariable?> FindByIdAndUserIdAsync(int processVariableId, int userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<ProcessVariable>()
            .FirstOrDefaultAsync(variable => variable.Id == processVariableId && variable.UserId == userId,
                cancellationToken);
    }

    public async Task<bool> ExistsByBatchIdAndTypeAsync(int batchId, string type, int userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<ProcessVariable>()
            .AnyAsync(variable => variable.BatchId == batchId && variable.Type == type && variable.UserId == userId,
                cancellationToken);
    }

    public async Task<ProcessVariable?> FindForReadingAsync(int userId, int? processVariableId, int? batchId,
        string? sensorId, CancellationToken cancellationToken = default)
    {
        var variables = Context.Set<ProcessVariable>().AsNoTracking().Where(variable => variable.UserId == userId);
        if (batchId is not null) variables = variables.Where(variable => variable.BatchId == batchId);
        if (processVariableId is not null) variables = variables.Where(variable => variable.Id == processVariableId);
        if (sensorId is not null) variables = variables.Where(variable => variable.SensorId == sensorId);
        return await variables.SingleOrDefaultAsync(cancellationToken);
    }
}
