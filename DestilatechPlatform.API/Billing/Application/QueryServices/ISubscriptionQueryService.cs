using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using DestilatechPlatform.API.Billing.Domain.Model.Queries;

namespace DestilatechPlatform.API.Billing.Application.QueryServices;

public interface ISubscriptionQueryService
{
    Task<IEnumerable<Subscription>> Handle(GetSubscriptionsByUserIdQuery query,
        CancellationToken cancellationToken = default);
}