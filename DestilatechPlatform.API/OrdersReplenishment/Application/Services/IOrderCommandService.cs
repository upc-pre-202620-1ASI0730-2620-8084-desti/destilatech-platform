using DestilatechPlatform.API.OrdersReplenishment.Domain.Model;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;
using DestilatechPlatform.API.Shared.Application.Patterns;

namespace DestilatechPlatform.API.OrdersReplenishment.Application.Services;

public interface IOrderCommandService
{
    Task<Result<Order, OrdersReplenishmentError>> Handle(RegisterOrderCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<Order, OrdersReplenishmentError>> Handle(AdvanceOrderStatusCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<Order, OrdersReplenishmentError>> Handle(CancelOrderCommand command,
        CancellationToken cancellationToken = default);
}
