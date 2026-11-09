using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using DestilatechPlatform.API.Billing.Domain.Repositories;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.Billing.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class SubscriptionRepository(AppDbContext context)
    : BaseRepository<Subscription>(context), ISubscriptionRepository
{
    public async Task<IEnumerable<Subscription>> ListByUserIdAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Subscription>()
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.StartDate)
            .ThenByDescending(s => s.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Subscription?> FindByPaymentReferenceAsync(string reference,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Subscription>()
            .FirstOrDefaultAsync(s => s.PaymentReference == reference, cancellationToken);
    }
}