using DestilatechPlatform.API.OrdersReplenishment.Application.Services;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.ValueObjects;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.ACL;

namespace DestilatechPlatform.API.OrdersReplenishment.Application.ACL;

public class OrdersContextFacade(IOrderQueryService orderQueryService) : IOrdersContextFacade
{
    public async Task<IReadOnlyCollection<OrderSalesSummary>> FetchOrdersPlacedSinceAsync(int userId,
        DateTimeOffset since, CancellationToken cancellationToken = default)
    {
        var orders = await orderQueryService.Handle(new GetAllOrdersByUserIdQuery(userId), cancellationToken);
        return orders
            .Where(order => order.OrderDate >= since)
            .Select(order => new OrderSalesSummary(order.Id, order.OrderDate, order.Total(),
                order.Status == OrderStatus.Cancelled))
            .ToList();
    }
}
