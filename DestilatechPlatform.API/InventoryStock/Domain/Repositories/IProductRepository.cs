using DestilatechPlatform.API.InventoryStock.Domain.Model.Aggregates;
using DestilatechPlatform.API.Shared.Domain.Repositories;

namespace DestilatechPlatform.API.InventoryStock.Domain.Repositories;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<IEnumerable<Product>> FindByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<Product?> FindByIdAndUserIdAsync(int productId, int userId, CancellationToken cancellationToken = default);
}
