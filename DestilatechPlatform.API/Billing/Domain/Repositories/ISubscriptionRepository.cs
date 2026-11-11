using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using DestilatechPlatform.API.Shared.Domain.Repositories;

namespace DestilatechPlatform.API.Billing.Domain.Repositories;

public interface ISubscriptionRepository : IBaseRepository<Subscription>
{
    // La más reciente primero, como espera el front
    Task<IEnumerable<Subscription>> ListByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<Subscription?> FindByPaymentReferenceAsync(string reference,
        CancellationToken cancellationToken = default);
}