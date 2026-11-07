using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;

namespace DestilatechPlatform.API.OrdersReplenishment.Application.Services;

public interface ICustomerQueryService
{
    Task<IEnumerable<Customer>> Handle(GetAllCustomersByUserIdQuery query,
        CancellationToken cancellationToken = default);

    Task<Customer?> Handle(GetCustomerByIdQuery query, CancellationToken cancellationToken = default);
}
