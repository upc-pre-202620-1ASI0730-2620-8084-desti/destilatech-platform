using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;

namespace DestilatechPlatform.API.OrdersReplenishment.Application.Services;

public interface IOrderQueryService
{
    Task<IEnumerable<Order>> HandleAsync(GetAllOrdersByUserIdQuery query, CancellationToken cancellationToken = default);

    Task<IEnumerable<Order>> HandleAsync(GetOrdersByCustomerIdQuery query,
        CancellationToken cancellationToken = default);

    Task<Order?> HandleAsync(GetOrderByIdQuery query, CancellationToken cancellationToken = default);
}
