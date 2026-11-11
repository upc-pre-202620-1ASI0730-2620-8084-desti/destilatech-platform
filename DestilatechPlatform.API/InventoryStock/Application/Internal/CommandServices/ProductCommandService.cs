using DestilatechPlatform.API.InventoryStock.Application.Services;
using DestilatechPlatform.API.InventoryStock.Domain.Model;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Aggregates;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Commands;
using DestilatechPlatform.API.InventoryStock.Domain.Repositories;
using DestilatechPlatform.API.Shared.Application.Model;
using DestilatechPlatform.API.Shared.Domain.Repositories;

namespace DestilatechPlatform.API.InventoryStock.Application.Internal.CommandServices;

public class ProductCommandService(
    IProductRepository productRepository,
    IStockItemRepository stockItemRepository,
    IUnitOfWork unitOfWork) : IProductCommandService
{
    public async Task<Result<Product>> Handle(CreateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var threshold = command.LowStockThreshold;
        if (threshold < 0 || threshold >= 1_000_000_000m || decimal.Round(threshold, 3) != threshold)
            return Result<Product>.Failure(InventoryStockError.InvalidData("Invalid low stock threshold."));

        Product product;
        try
        {
            product = new Product(command.UserId, command.Name, command.Presentation, command.Unit,
                command.Category, command.UnitPrice, command.Sku);
        }
        catch (ArgumentException exception)
        {
            return Result<Product>.Failure(InventoryStockError.InvalidData(exception.Message));
        }

        // The stock item needs the product id, so the product is saved first.
        await productRepository.AddAsync(product, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        await stockItemRepository.AddAsync(new StockItem(product.Id, product.UserId, threshold), cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<Product>.Success(product);
    }

    public async Task<Result<Product>> Handle(UpdateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.FindByIdAndUserIdAsync(command.ProductId, command.UserId,
            cancellationToken);
        if (product is null)
            return Result<Product>.Failure(InventoryStockError.ProductNotFound(command.ProductId));

        try
        {
            product.Edit(command.Name, command.Presentation, command.Unit, command.Category, command.UnitPrice,
                command.Sku);
        }
        catch (ArgumentException exception)
        {
            return Result<Product>.Failure(InventoryStockError.InvalidData(exception.Message));
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<Product>.Success(product);
    }
}
