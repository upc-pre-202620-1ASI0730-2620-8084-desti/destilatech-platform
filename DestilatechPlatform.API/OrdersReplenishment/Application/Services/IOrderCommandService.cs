using DestilatechPlatform.API.OrdersReplenishment.Domain.Model;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;
using DestilatechPlatform.API.Shared.Application.Model;

namespace DestilatechPlatform.API.OrdersReplenishment.Application.Services;

public interface IOrderCommandService
{
    Task<Result<Order>> Handle(RegisterOrderCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<Order>> Handle(AdvanceOrderStatusCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<Order>> Handle(CancelOrderCommand command,
        CancellationToken cancellationToken = default);
}
