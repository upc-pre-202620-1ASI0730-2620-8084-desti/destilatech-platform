using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Repositories;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.OrdersReplenishment.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class CustomerRepository(AppDbContext context) : BaseRepository<Customer>(context), ICustomerRepository
{
    public async Task<IEnumerable<Customer>> FindByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Customer>()
            .Where(customer => customer.UserId == userId)
            .ToListAsync(cancellationToken);
    }
}
