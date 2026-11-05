using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;

namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Services;

public interface IReplenishmentOrderQueryService
{
    Task<IEnumerable<ReplenishmentOrder>> HandleAsync(GetAllReplenishmentOrdersByUserIdQuery query,
        CancellationToken cancellationToken = default);

    Task<ReplenishmentOrder?> HandleAsync(GetReplenishmentOrderByIdQuery query,
        CancellationToken cancellationToken = default);
}
