using DestilatechPlatform.API.OrdersReplenishment.Domain.Model;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;
using DestilatechPlatform.API.Shared.Application.Patterns;

namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Services;

public interface IOrderCommandService
{
    Task<Result<Order, OrdersReplenishmentError>> HandleAsync(RegisterOrderCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<Order, OrdersReplenishmentError>> HandleAsync(AdvanceOrderStatusCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<Order, OrdersReplenishmentError>> HandleAsync(CancelOrderCommand command,
        CancellationToken cancellationToken = default);
}
