using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Aggregates;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Repositories;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.AnalyticsEstimations.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
/// Repository of <see cref="ReplenishmentEstimate"/>. Estimates are never updated: every calculation adds a
/// new row, and the latest one of a product is the one with the highest id.
/// </summary>
public class ReplenishmentEstimateRepository(AppDbContext context)
    : BaseRepository<ReplenishmentEstimate>(context), IReplenishmentEstimateRepository
{
    public async Task<ReplenishmentEstimate?> FindLatestByProductIdAsync(int userId, int productId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<ReplenishmentEstimate>()
            .AsNoTracking()
            .Where(estimate => estimate.UserId == userId && estimate.ProductId == productId)
            .OrderByDescending(estimate => estimate.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<ReplenishmentEstimate>> FindLatestByUserIdAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        var estimates = Context.Set<ReplenishmentEstimate>().AsNoTracking();
        return await estimates
            .Where(estimate => estimate.UserId == userId
                               && estimate.Id == estimates
                                   .Where(other => other.UserId == userId && other.ProductId == estimate.ProductId)
                                   .Max(other => other.Id))
            .OrderBy(estimate => estimate.ProductId)
            .ToListAsync(cancellationToken);
    }
}
