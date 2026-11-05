using DestilatechPlatform.API.OrdersReplenishment.Domain.Model;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;
using DestilatechPlatform.API.Shared.Application.Patterns;

namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Services;

public interface ICustomerCommandService
{
    Task<Result<Customer, OrdersReplenishmentError>> HandleAsync(RegisterCustomerCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<Customer, OrdersReplenishmentError>> HandleAsync(UpdateCustomerCommand command,
        CancellationToken cancellationToken = default);
}
