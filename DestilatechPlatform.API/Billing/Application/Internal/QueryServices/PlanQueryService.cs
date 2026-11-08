using DestilatechPlatform.API.Billing.Application.QueryServices;
using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using DestilatechPlatform.API.Billing.Domain.Model.Queries;
using DestilatechPlatform.API.Billing.Domain.Repositories;

namespace DestilatechPlatform.API.Billing.Application.Internal.QueryServices;

public class PlanQueryService(IPlanRepository planRepository) : IPlanQueryService
{
    public async Task<IEnumerable<Plan>> Handle(GetAllPlansQuery query,
        CancellationToken cancellationToken = default)
    {
        var plans = await planRepository.ListAsync(cancellationToken);
        return plans.OrderBy(p => p.Price);
    }
}