using DestilatechPlatform.API.OrdersReplenishment.Domain.Model;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;
using DestilatechPlatform.API.Shared.Application.Patterns;

namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Services;

public interface IReplenishmentOrderCommandService
{
    Task<Result<ReplenishmentOrder, OrdersReplenishmentError>> HandleAsync(RequestReplenishmentOrderCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<ReplenishmentOrder, OrdersReplenishmentError>> HandleAsync(ReceiveReplenishmentOrderCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<ReplenishmentOrder, OrdersReplenishmentError>> HandleAsync(CancelReplenishmentOrderCommand command,
        CancellationToken cancellationToken = default);
}
