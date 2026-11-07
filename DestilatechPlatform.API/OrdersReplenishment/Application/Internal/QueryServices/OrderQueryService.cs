using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Repositories;
using DestilatechPlatform.API.OrdersReplenishment.Application.Services;

namespace DestilatechPlatform.API.OrdersReplenishment.Application.Internal.QueryServices;

public class OrderQueryService(IOrderRepository orderRepository) : IOrderQueryService
{
    public async Task<IEnumerable<Order>> Handle(GetAllOrdersByUserIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var orders = await orderRepository.FindByUserIdAsync(query.UserId, cancellationToken);
        return orders
            .OrderByDescending(order => order.OrderDate)
            .ThenByDescending(order => order.Id);
    }

    public async Task<IEnumerable<Order>> Handle(GetOrdersByCustomerIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var orders = await orderRepository.FindByCustomerIdAsync(query.CustomerId, cancellationToken);
        return orders
            .Where(order => order.BelongsTo(query.UserId))
            .OrderByDescending(order => order.OrderDate)
            .ThenByDescending(order => order.Id);
    }

    public async Task<Order?> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.FindByIdAsync(query.OrderId, cancellationToken);
        return order is not null && order.BelongsTo(query.UserId) ? order : null;
    }
}
