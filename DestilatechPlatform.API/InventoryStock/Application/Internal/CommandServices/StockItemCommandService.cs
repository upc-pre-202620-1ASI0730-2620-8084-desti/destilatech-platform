using DestilatechPlatform.API.AlertsNotifications.Interfaces.ACL;
using DestilatechPlatform.API.InventoryStock.Application.Services;
using DestilatechPlatform.API.InventoryStock.Domain.Model;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Aggregates;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Commands;
using DestilatechPlatform.API.InventoryStock.Domain.Model.Entities;
using DestilatechPlatform.API.InventoryStock.Domain.Repositories;
using DestilatechPlatform.API.Shared.Application.Model;
using DestilatechPlatform.API.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.InventoryStock.Application.Internal.CommandServices;

public class StockItemCommandService(
    IProductRepository productRepository,
    IStockItemRepository stockItemRepository,
    IStockMovementRepository stockMovementRepository,
    IUnitOfWork unitOfWork,
    IAlertsContextFacade alertsContextFacade) : IStockItemCommandService
{
    // Production, orders and returns register their own movements through the inventory facade.
    private static readonly string[] SourceWorkflowReasons = ["PRODUCTION", "ORDER", "RETURN"];

    public async Task<Result<StockItem>> Handle(ConfigureLowStockThresholdCommand command,
        CancellationToken cancellationToken = default)
    {
        var stockItem = await stockItemRepository.FindByIdAndUserIdAsync(command.StockItemId, command.UserId,
            cancellationToken);
        if (stockItem is null)
            return Result<StockItem>.Failure(InventoryStockError.StockItemNotFound(command.StockItemId));

        try
        {
            stockItem.ConfigureThreshold(command.LowStockThreshold);
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (ArgumentException exception)
        {
            return Result<StockItem>.Failure(InventoryStockError.InvalidData(exception.Message));
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<StockItem>.Failure(InventoryStockError.StockChanged());
        }

        await RaiseLowStockAlertIfNeededAsync(stockItem, cancellationToken);
        return Result<StockItem>.Success(stockItem);
    }

    public async Task<Result<StockMovement>> Handle(RegisterStockMovementCommand command,
        CancellationToken cancellationToken = default)
    {
        if (SourceWorkflowReasons.Contains(command.Reason))
            return Result<StockMovement>.Failure(InventoryStockError.SourceWorkflowRequired());

        var product = await productRepository.FindByIdAndUserIdAsync(command.ProductId, command.UserId,
            cancellationToken);
        var stockItem = await stockItemRepository.FindByProductIdAndUserIdAsync(command.ProductId, command.UserId,
            cancellationToken);
        if (product is not { IsActive: true } || stockItem is null)
            return Result<StockMovement>.Failure(InventoryStockError.ProductNotFound(command.ProductId));

        StockMovement movement;
        try
        {
            movement = stockItem.RegisterMovement(command.Type, command.Reason, command.Quantity, command.Notes,
                command.Reference);
        }
        catch (ArgumentException exception)
        {
            return Result<StockMovement>.Failure(InventoryStockError.InvalidData(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return Result<StockMovement>.Failure(exception.Message == "Insufficient stock."
                ? InventoryStockError.InsufficientStock()
                : InventoryStockError.MaximumStockExceeded());
        }

        try
        {
            await stockMovementRepository.AddAsync(movement, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<StockMovement>.Failure(InventoryStockError.StockChanged());
        }

        await RaiseLowStockAlertIfNeededAsync(stockItem, cancellationToken);
        return Result<StockMovement>.Success(movement);
    }

    private async Task RaiseLowStockAlertIfNeededAsync(StockItem stockItem, CancellationToken cancellationToken)
    {
        if (stockItem.IsBelowThreshold())
            await alertsContextFacade.RaiseLowStockAlert(stockItem.UserId, stockItem.ProductId,
                $"Product {stockItem.ProductId} is at or below its stock threshold.", cancellationToken);
    }
}
