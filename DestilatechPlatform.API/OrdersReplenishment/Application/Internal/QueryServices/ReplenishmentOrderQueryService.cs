using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Repositories;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Services;

namespace DestilatechPlatform.API.OrdersReplenishment.Application.Internal.QueryServices;

public class ReplenishmentOrderQueryService(IReplenishmentOrderRepository replenishmentOrderRepository)
    : IReplenishmentOrderQueryService
{
    public async Task<IEnumerable<ReplenishmentOrder>> HandleAsync(GetAllReplenishmentOrdersByUserIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var replenishmentOrders = await replenishmentOrderRepository.FindByUserIdAsync(query.UserId,
            cancellationToken);
        return replenishmentOrders.OrderByDescending(replenishmentOrder => replenishmentOrder.OrderDate);
    }

    public async Task<ReplenishmentOrder?> HandleAsync(GetReplenishmentOrderByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var replenishmentOrder = await replenishmentOrderRepository.FindByIdAsync(query.ReplenishmentOrderId,
            cancellationToken);
        return replenishmentOrder is not null && replenishmentOrder.BelongsTo(query.UserId)
            ? replenishmentOrder
            : null;
    }
}
