using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;

namespace DestilatechPlatform.API.OrdersReplenishment.Application.Services;

public interface IOrderQueryService
{
    Task<IEnumerable<Order>> Handle(GetAllOrdersByUserIdQuery query, CancellationToken cancellationToken = default);

    Task<IEnumerable<Order>> Handle(GetOrdersByCustomerIdQuery query,
        CancellationToken cancellationToken = default);

    Task<Order?> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken = default);
}
