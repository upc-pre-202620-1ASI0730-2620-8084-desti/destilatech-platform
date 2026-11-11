using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using DestilatechPlatform.API.Billing.Domain.Repositories;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.Billing.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class PaymentRepository(AppDbContext context) : BaseRepository<Payment>(context), IPaymentRepository
{
    public async Task<IEnumerable<Payment>> ListByUserIdAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Payment>()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.PaidAt)
            .ThenByDescending(p => p.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Payment?> FindByReferenceAsync(string reference,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Payment>()
            .FirstOrDefaultAsync(p => p.Reference == reference, cancellationToken);
    }
}