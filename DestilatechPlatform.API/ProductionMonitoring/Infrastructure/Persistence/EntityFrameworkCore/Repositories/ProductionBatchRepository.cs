using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Repositories;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.ProductionMonitoring.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class ProductionBatchRepository(AppDbContext context)
    : BaseRepository<ProductionBatch>(context), IProductionBatchRepository
{
    public async Task<IEnumerable<ProductionBatch>> FindByUserIdAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<ProductionBatch>()
            .AsNoTracking()
            .Include(batch => batch.StageHistory)
            .Where(batch => batch.UserId == userId)
            .OrderByDescending(batch => batch.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductionBatch?> FindByIdAndUserIdAsync(int batchId, int userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<ProductionBatch>()
            .Include(batch => batch.StageHistory)
            .FirstOrDefaultAsync(batch => batch.Id == batchId && batch.UserId == userId, cancellationToken);
    }

    public async Task<bool> ExistsByIdAndUserIdAsync(int batchId, int userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<ProductionBatch>()
            .AnyAsync(batch => batch.Id == batchId && batch.UserId == userId, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAndUserIdAsync(string code, int userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<ProductionBatch>()
            .AnyAsync(batch => batch.Code == code && batch.UserId == userId, cancellationToken);
    }
}
