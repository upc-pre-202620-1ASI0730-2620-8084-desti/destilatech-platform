using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;

namespace DestilatechPlatform.API.OrdersReplenishment.Application.Services;

public interface IReplenishmentOrderQueryService
{
    Task<IEnumerable<ReplenishmentOrder>> Handle(GetAllReplenishmentOrdersByUserIdQuery query,
        CancellationToken cancellationToken = default);

    Task<ReplenishmentOrder?> Handle(GetReplenishmentOrderByIdQuery query,
        CancellationToken cancellationToken = default);
}
