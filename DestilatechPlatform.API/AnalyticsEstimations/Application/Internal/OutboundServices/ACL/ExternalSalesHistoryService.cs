using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.ACL;

namespace DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.OutboundServices.ACL;

/// <summary>
/// Adapter of <see cref="ISalesHistoryReadPort"/> over the facade of the Orders &amp; Replenishment bounded context.
/// </summary>
public class ExternalSalesHistoryService(IOrdersContextFacade ordersContextFacade) : ISalesHistoryReadPort
{
    public async Task<IReadOnlyCollection<SaleRecord>> ReadSalesAsync(int userId, DateTimeOffset since,
        CancellationToken cancellationToken = default)
    {
        var orders = await ordersContextFacade.FetchOrdersPlacedSinceAsync(userId, since, cancellationToken);
        return orders
            .Select(order => new SaleRecord(order.OrderDate, order.Total, order.Cancelled))
            .ToList();
    }
}
