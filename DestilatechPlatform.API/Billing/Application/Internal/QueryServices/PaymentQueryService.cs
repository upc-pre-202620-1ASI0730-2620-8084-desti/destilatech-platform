using DestilatechPlatform.API.Billing.Application.QueryServices;
using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using DestilatechPlatform.API.Billing.Domain.Model.Queries;
using DestilatechPlatform.API.Billing.Domain.Repositories;

namespace DestilatechPlatform.API.Billing.Application.Internal.QueryServices;

public class PaymentQueryService(IPaymentRepository paymentRepository) : IPaymentQueryService
{
    public async Task<IEnumerable<Payment>> Handle(GetPaymentsByUserIdQuery query,
        CancellationToken cancellationToken = default)
    {
        return await paymentRepository.ListByUserIdAsync(query.UserId, cancellationToken);
    }

    public async Task<Payment?> Handle(GetPaymentByReferenceQuery query,
        CancellationToken cancellationToken = default)
    {
        return await paymentRepository.FindByReferenceAsync(query.Reference, cancellationToken);
    }
}