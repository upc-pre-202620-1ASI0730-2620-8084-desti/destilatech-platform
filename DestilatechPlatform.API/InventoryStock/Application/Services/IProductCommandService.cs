using DestilatechPlatform.API.InventoryStock.Domain.Model.Aggregates;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Commands;
using DestilatechPlatform.API.Shared.Application.Model;

namespace DestilatechPlatform.API.InventoryStock.Application.Services;

public interface IProductCommandService
{
    Task<Result<Product>> Handle(CreateProductCommand command, CancellationToken cancellationToken = default);

    Task<Result<Product>> Handle(UpdateProductCommand command, CancellationToken cancellationToken = default);
}
