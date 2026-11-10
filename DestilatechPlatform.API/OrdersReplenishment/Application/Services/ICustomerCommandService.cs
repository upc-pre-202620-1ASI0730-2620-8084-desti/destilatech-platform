using DestilatechPlatform.API.OrdersReplenishment.Domain.Model;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;
using DestilatechPlatform.API.Shared.Application.Model;

namespace DestilatechPlatform.API.OrdersReplenishment.Application.Services;

public interface ICustomerCommandService
{
    Task<Result<Customer>> Handle(RegisterCustomerCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<Customer>> Handle(UpdateCustomerCommand command,
        CancellationToken cancellationToken = default);
}
