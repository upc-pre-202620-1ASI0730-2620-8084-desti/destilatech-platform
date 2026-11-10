using DestilatechPlatform.API.OrdersReplenishment.Domain.Model;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;
using DestilatechPlatform.API.Shared.Application.Model;

namespace DestilatechPlatform.API.OrdersReplenishment.Application.Services;

public interface IReplenishmentOrderCommandService
{
    Task<Result<ReplenishmentOrder>> Handle(RequestReplenishmentOrderCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<ReplenishmentOrder>> Handle(ReceiveReplenishmentOrderCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<ReplenishmentOrder>> Handle(CancelReplenishmentOrderCommand command,
        CancellationToken cancellationToken = default);
}
