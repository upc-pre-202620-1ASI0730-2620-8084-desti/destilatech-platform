using DestilatechPlatform.API.InventoryStock.Application;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.OrdersReplenishment.Application.Internal.OutboundServices.ACL;

/// <summary>
/// Adapter of <see cref="IInventoryPort"/> over the Inventory &amp; Stock Management bounded context.
/// </summary>
/// <remarks>
/// Stock movements are only staged in the shared context through
/// <see cref="InventoryService.StageOrderMovementAsync"/>; the command service saves them together
/// with the order, so both are committed in the same transaction.
/// </remarks>
public class ExternalInventoryService(InventoryService inventoryService) : IInventoryPort
{
    private const string MovementIn = "IN";
    private const string MovementOut = "OUT";
    private const string ReasonOrder = "ORDER";
    private const string ReasonReturn = "RETURN";
    private const string ReasonPurchase = "PURCHASE";

    public async Task<InventoryProduct?> FindProductAsync(int userId, int productId,
        CancellationToken cancellationToken = default)
    {
        var product = await inventoryService.Products(userId)
            .Where(item => item.Id == productId && item.IsActive)
            .Select(item => new { item.Id, item.Name, item.Presentation, item.UnitPrice })
            .FirstOrDefaultAsync(cancellationToken);
        if (product is null)
        {
            return null;
        }

        var availableQuantity = await inventoryService.StockItems(userId)
            .Where(stock => stock.ProductId == productId)
            .Select(stock => (decimal?)stock.CurrentQuantity)
            .FirstOrDefaultAsync(cancellationToken) ?? 0m;

        // Same display name as the web application: "Name · Presentation".
        var displayName = string.IsNullOrWhiteSpace(product.Presentation)
            ? product.Name
            : $"{product.Name} · {product.Presentation}";
        return new InventoryProduct(product.Id, displayName, product.UnitPrice, availableQuantity);
    }

    public async Task RegisterOrderExitAsync(int userId, int productId, int quantity, string reference,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await inventoryService.StageOrderMovementAsync(userId, productId, MovementOut, ReasonOrder, quantity,
                reference, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            throw new OrdersReplenishmentException(OrdersReplenishmentError.InsufficientStock());
        }
    }

    public async Task RegisterEntryAsync(int userId, int productId, int quantity, StockEntryReason reason,
        string reference, CancellationToken cancellationToken = default)
    {
        var inventoryReason = reason == StockEntryReason.Purchase ? ReasonPurchase : ReasonReturn;
        try
        {
            await inventoryService.StageOrderMovementAsync(userId, productId, MovementIn, inventoryReason, quantity,
                reference, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            throw new OrdersReplenishmentException(OrdersReplenishmentError.ProductRequired());
        }
    }
}
