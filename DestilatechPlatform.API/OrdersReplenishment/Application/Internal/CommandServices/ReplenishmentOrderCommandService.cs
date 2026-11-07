using DestilatechPlatform.API.OrdersReplenishment.Application.Internal.OutboundServices;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Repositories;
using DestilatechPlatform.API.OrdersReplenishment.Application.Services;
using DestilatechPlatform.API.Shared.Domain.Repositories;
using ReplenishmentResult = DestilatechPlatform.API.Shared.Application.Patterns.Result<
    DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates.ReplenishmentOrder,
    DestilatechPlatform.API.OrdersReplenishment.Domain.Model.OrdersReplenishmentError>;

namespace DestilatechPlatform.API.OrdersReplenishment.Application.Internal.CommandServices;

/// <summary>
/// Requests, receives and cancels replenishment orders placed with suppliers.
/// Receiving an order adds its units to stock in the same unit of work.
/// </summary>
public class ReplenishmentOrderCommandService(
    IReplenishmentOrderRepository replenishmentOrderRepository,
    IInventoryPort inventoryPort,
    IUnitOfWork unitOfWork) : IReplenishmentOrderCommandService
{
    public async Task<ReplenishmentResult> HandleAsync(RequestReplenishmentOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var product = await inventoryPort.FindProductAsync(command.UserId, command.ProductId, cancellationToken);
        if (product is null)
        {
            return new ReplenishmentResult.Failure(OrdersReplenishmentError.ProductRequired());
        }

        var sequence = await replenishmentOrderRepository.CountByUserIdAsync(command.UserId, cancellationToken) + 1;
        ReplenishmentOrder replenishmentOrder;
        try
        {
            replenishmentOrder = new ReplenishmentOrder(command, ReplenishmentOrder.CodeFor(sequence),
                product.DisplayName);
        }
        catch (OrdersReplenishmentException exception)
        {
            return new ReplenishmentResult.Failure(exception.Error);
        }

        await replenishmentOrderRepository.AddAsync(replenishmentOrder, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new ReplenishmentResult.Success(replenishmentOrder);
    }

    public async Task<ReplenishmentResult> HandleAsync(ReceiveReplenishmentOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var replenishmentOrder =
            await FindOwnedReplenishmentOrderAsync(command.ReplenishmentOrderId, command.UserId, cancellationToken);
        if (replenishmentOrder is null)
        {
            return new ReplenishmentResult.Failure(
                OrdersReplenishmentError.ReplenishmentOrderNotFound(command.ReplenishmentOrderId));
        }

        try
        {
            replenishmentOrder.Receive();
        }
        catch (OrdersReplenishmentException exception)
        {
            return new ReplenishmentResult.Failure(exception.Error);
        }

        replenishmentOrderRepository.Update(replenishmentOrder);
        await inventoryPort.RegisterEntryAsync(replenishmentOrder.UserId, replenishmentOrder.ProductId,
            replenishmentOrder.Quantity, StockEntryReason.Purchase, replenishmentOrder.Code, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new ReplenishmentResult.Success(replenishmentOrder);
    }

    public async Task<ReplenishmentResult> HandleAsync(CancelReplenishmentOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var replenishmentOrder =
            await FindOwnedReplenishmentOrderAsync(command.ReplenishmentOrderId, command.UserId, cancellationToken);
        if (replenishmentOrder is null)
        {
            return new ReplenishmentResult.Failure(
                OrdersReplenishmentError.ReplenishmentOrderNotFound(command.ReplenishmentOrderId));
        }

        try
        {
            replenishmentOrder.Cancel();
        }
        catch (OrdersReplenishmentException exception)
        {
            return new ReplenishmentResult.Failure(exception.Error);
        }

        replenishmentOrderRepository.Update(replenishmentOrder);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new ReplenishmentResult.Success(replenishmentOrder);
    }

    private async Task<ReplenishmentOrder?> FindOwnedReplenishmentOrderAsync(int replenishmentOrderId, int userId,
        CancellationToken cancellationToken)
    {
        var replenishmentOrder = await replenishmentOrderRepository.FindByIdAsync(replenishmentOrderId,
            cancellationToken);
        return replenishmentOrder is not null && replenishmentOrder.BelongsTo(userId) ? replenishmentOrder : null;
    }
}
