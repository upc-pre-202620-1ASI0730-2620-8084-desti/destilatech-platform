using DestilatechPlatform.API.InventoryStock.Domain.Model.Aggregates;
using DestilatechPlatform.API.InventoryStock.Domain.Repositories;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.InventoryStock.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class ProductRepository(AppDbContext context) : BaseRepository<Product>(context), IProductRepository
{
    public async Task<IEnumerable<Product>> FindByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Product>()
            .Where(product => product.UserId == userId)
            .OrderBy(product => product.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> FindByIdAndUserIdAsync(int productId, int userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Product>()
            .FirstOrDefaultAsync(product => product.Id == productId && product.UserId == userId, cancellationToken);
    }
}
