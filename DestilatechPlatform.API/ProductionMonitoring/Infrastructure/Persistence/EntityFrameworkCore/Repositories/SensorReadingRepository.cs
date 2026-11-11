using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Entities;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Repositories;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.ProductionMonitoring.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class SensorReadingRepository(AppDbContext context)
    : BaseRepository<SensorReading>(context), ISensorReadingRepository
{
    private const int MaxBatchReadings = 1000;

    public async Task<IEnumerable<SensorReading>> FindLatestByUserIdAsync(int userId, int limit,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<SensorReading>()
            .AsNoTracking()
            .Where(reading => reading.UserId == userId)
            .OrderByDescending(reading => reading.RecordedAt)
            .ThenByDescending(reading => reading.Id)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<SensorReading?> FindBySourceKeyAsync(int userId, int processVariableId, string sourceKey,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<SensorReading>()
            .AsNoTracking()
            .FirstOrDefaultAsync(reading => reading.UserId == userId &&
                                            reading.ProcessVariableId == processVariableId &&
                                            reading.SourceKey == sourceKey, cancellationToken);
    }

    public async Task<IEnumerable<SensorReading>> FindByBatchIdAsync(int userId, int batchId, string? variableType,
        DateTime? fromUtc, DateTime? toUtc, CancellationToken cancellationToken = default)
    {
        var readings = Context.Set<SensorReading>()
            .AsNoTracking()
            .Where(reading => reading.UserId == userId && reading.BatchId == batchId);
        if (variableType is not null)
        {
            var variableIds = Context.Set<ProcessVariable>()
                .Where(variable => variable.UserId == userId && variable.BatchId == batchId &&
                                   variable.Type == variableType)
                .Select(variable => variable.Id);
            readings = readings.Where(reading => variableIds.Contains(reading.ProcessVariableId));
        }

        if (fromUtc is not null) readings = readings.Where(reading => reading.RecordedAt >= fromUtc);
        if (toUtc is not null) readings = readings.Where(reading => reading.RecordedAt <= toUtc);
        return await readings
            .OrderBy(reading => reading.RecordedAt)
            .ThenBy(reading => reading.Id)
            .Take(MaxBatchReadings)
            .ToListAsync(cancellationToken);
    }
}
