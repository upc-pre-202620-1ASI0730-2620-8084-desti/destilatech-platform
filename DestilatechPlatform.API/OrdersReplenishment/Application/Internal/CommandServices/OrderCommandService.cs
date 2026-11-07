using DestilatechPlatform.API.OrdersReplenishment.Application.Internal.OutboundServices;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Entities;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Repositories;
using DestilatechPlatform.API.OrdersReplenishment.Application.Services;
using DestilatechPlatform.API.Shared.Domain.Repositories;
using OrderResult = DestilatechPlatform.API.Shared.Application.Patterns.Result<
    DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates.Order,
    DestilatechPlatform.API.OrdersReplenishment.Domain.Model.OrdersReplenishmentError>;

namespace DestilatechPlatform.API.OrdersReplenishment.Application.Internal.CommandServices;

/// <summary>
/// Registers orders and moves them through their lifecycle (US18). Stock is discounted when an
/// order is registered and returned when it is cancelled, in the same unit of work as the order.
/// </summary>
public class OrderCommandService(
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    IInventoryPort inventoryPort,
    IUnitOfWork unitOfWork) : IOrderCommandService
{
    public async Task<OrderResult> HandleAsync(RegisterOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.Lines is null || command.Lines.Count == 0)
        {
            return new OrderResult.Failure(OrdersReplenishmentError.LinesRequired());
        }

        var customer = await customerRepository.FindByIdAsync(command.CustomerId, cancellationToken);
        if (customer is null || !customer.BelongsTo(command.UserId))
        {
            return new OrderResult.Failure(OrdersReplenishmentError.CustomerRequired());
        }

        var lines = new List<OrderLine>();
        try
        {
            // The same product may come in several lines, so stock is checked against the total requested.
            foreach (var requested in command.Lines.GroupBy(line => line.ProductId))
            {
                var product = await inventoryPort.FindProductAsync(command.UserId, requested.Key, cancellationToken);
                if (product is null)
                {
                    return new OrderResult.Failure(OrdersReplenishmentError.ProductRequired());
                }

                if (requested.Sum(line => line.Quantity) > product.AvailableQuantity)
                {
                    return new OrderResult.Failure(OrdersReplenishmentError.InsufficientStock());
                }

                lines.AddRange(requested.Select(line =>
                    new OrderLine(product.ProductId, product.DisplayName, line.Quantity, product.UnitPrice)));
            }

            var existingCodes = await orderRepository.FindCodesByUserIdAsync(command.UserId, cancellationToken);
            var order = new Order(command.UserId, Order.NextCode(existingCodes), customer.Id, customer.Name, lines,
                command.Notes);

            await orderRepository.AddAsync(order, cancellationToken);
            foreach (var line in order.Lines)
            {
                await inventoryPort.RegisterOrderExitAsync(order.UserId, line.ProductId, line.Quantity, order.Code,
                    cancellationToken);
            }

            await unitOfWork.CompleteAsync(cancellationToken);
            return new OrderResult.Success(order);
        }
        catch (OrdersReplenishmentException exception)
        {
            return new OrderResult.Failure(exception.Error);
        }
    }

    public async Task<OrderResult> HandleAsync(AdvanceOrderStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await FindOwnedOrderAsync(command.OrderId, command.UserId, cancellationToken);
        if (order is null)
        {
            return new OrderResult.Failure(OrdersReplenishmentError.OrderNotFound(command.OrderId));
        }

        try
        {
            order.AdvanceStatus();
        }
        catch (OrdersReplenishmentException exception)
        {
            return new OrderResult.Failure(exception.Error);
        }

        orderRepository.Update(order);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new OrderResult.Success(order);
    }

    public async Task<OrderResult> HandleAsync(CancelOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await FindOwnedOrderAsync(command.OrderId, command.UserId, cancellationToken);
        if (order is null)
        {
            return new OrderResult.Failure(OrdersReplenishmentError.OrderNotFound(command.OrderId));
        }

        try
        {
            order.Cancel();
        }
        catch (OrdersReplenishmentException exception)
        {
            return new OrderResult.Failure(exception.Error);
        }

        orderRepository.Update(order);
        foreach (var line in order.Lines)
        {
            await inventoryPort.RegisterEntryAsync(order.UserId, line.ProductId, line.Quantity,
                StockEntryReason.Return, order.Code, cancellationToken);
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return new OrderResult.Success(order);
    }

    /// <summary>
    /// Finds an order only if it belongs to the account, so one account cannot change another's data.
    /// </summary>
    private async Task<Order?> FindOwnedOrderAsync(int orderId, int userId, CancellationToken cancellationToken)
    {
        var order = await orderRepository.FindByIdAsync(orderId, cancellationToken);
        return order is not null && order.BelongsTo(userId) ? order : null;
    }
}
