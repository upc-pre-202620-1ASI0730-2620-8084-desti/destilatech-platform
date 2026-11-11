using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using DestilatechPlatform.API.Shared.Domain.Repositories;

namespace DestilatechPlatform.API.Billing.Domain.Repositories;

public interface IPaymentRepository : IBaseRepository<Payment>
{
// El más reciente primero, como espera el front
    Task<IEnumerable<Payment>> ListByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<Payment?> FindByReferenceAsync(string reference, CancellationToken cancellationToken = default);
}