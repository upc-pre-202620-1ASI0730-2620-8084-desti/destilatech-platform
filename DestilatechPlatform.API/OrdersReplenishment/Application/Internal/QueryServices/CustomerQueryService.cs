using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Repositories;
using DestilatechPlatform.API.OrdersReplenishment.Application.Services;

namespace DestilatechPlatform.API.OrdersReplenishment.Application.Internal.QueryServices;

public class CustomerQueryService(ICustomerRepository customerRepository) : ICustomerQueryService
{
    public async Task<IEnumerable<Customer>> HandleAsync(GetAllCustomersByUserIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var customers = await customerRepository.FindByUserIdAsync(query.UserId, cancellationToken);
        return customers.OrderBy(customer => customer.Name, StringComparer.CurrentCultureIgnoreCase);
    }

    public async Task<Customer?> HandleAsync(GetCustomerByIdQuery query, CancellationToken cancellationToken = default)
    {
        var customer = await customerRepository.FindByIdAsync(query.CustomerId, cancellationToken);
        return customer is not null && customer.BelongsTo(query.UserId) ? customer : null;
    }
}
