using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;

namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Services;

public interface ICustomerQueryService
{
    Task<IEnumerable<Customer>> HandleAsync(GetAllCustomersByUserIdQuery query,
        CancellationToken cancellationToken = default);

    Task<Customer?> HandleAsync(GetCustomerByIdQuery query, CancellationToken cancellationToken = default);
}
