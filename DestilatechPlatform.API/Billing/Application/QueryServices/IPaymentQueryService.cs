using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using DestilatechPlatform.API.Billing.Domain.Model.Queries;

namespace DestilatechPlatform.API.Billing.Application.QueryServices;

public interface IPaymentQueryService
{
    Task<IEnumerable<Payment>> Handle(GetPaymentsByUserIdQuery query,
        CancellationToken cancellationToken = default);

    Task<Payment?> Handle(GetPaymentByReferenceQuery query, CancellationToken cancellationToken = default);
}