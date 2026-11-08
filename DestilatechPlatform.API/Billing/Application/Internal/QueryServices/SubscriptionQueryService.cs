using DestilatechPlatform.API.Billing.Application.QueryServices;
using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using DestilatechPlatform.API.Billing.Domain.Model.Queries;
using DestilatechPlatform.API.Billing.Domain.Repositories;

namespace DestilatechPlatform.API.Billing.Application.Internal.QueryServices;

public class SubscriptionQueryService(ISubscriptionRepository subscriptionRepository) : ISubscriptionQueryService
{
    public async Task<IEnumerable<Subscription>> Handle(GetSubscriptionsByUserIdQuery query,
        CancellationToken cancellationToken = default)
    {
        return await subscriptionRepository.ListByUserIdAsync(query.UserId, cancellationToken);
    }
}