using DestilatechPlatform.API.InventoryStock.Interfaces.ACL;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model;

namespace DestilatechPlatform.API.OrdersReplenishment.Application.Internal.OutboundServices.ACL;

/// <summary>
/// Adapter of <see cref="IInventoryPort"/> over the Inventory &amp; Stock Management bounded context.
/// </summary>
/// <remarks>
/// Stock movements are only staged in the shared unit of work through
/// <see cref="IInventoryContextFacade.StageStockMovementAsync"/>; the command service saves them together
/// with the order, so both are committed in the same transaction.
/// </remarks>
public class ExternalInventoryService(IInventoryContextFacade inventoryContextFacade) : IInventoryPort
{
    private const string MovementIn = "IN";
    private const string MovementOut = "OUT";
    private const string ReasonOrder = "ORDER";
    private const string ReasonReturn = "RETURN";
    private const string ReasonPurchase = "PURCHASE";

    public async Task<InventoryProduct?> FindProductAsync(int userId, int productId,
        CancellationToken cancellationToken = default)
    {
        var product = await inventoryContextFacade.FetchActiveProductAsync(userId, productId, cancellationToken);
        if (product is null)
        {
            return null;
        }

        // Same display name as the web application: "Name · Presentation".
        var displayName = string.IsNullOrWhiteSpace(product.Presentation)
            ? product.Name
            : $"{product.Name} · {product.Presentation}";
        return new InventoryProduct(product.ProductId, displayName, product.UnitPrice, product.AvailableQuantity);
    }

    public async Task RegisterOrderExitAsync(int userId, int productId, int quantity, string reference,
        CancellationToken cancellationToken = default)
    {
        var staged = await inventoryContextFacade.StageStockMovementAsync(userId, productId, MovementOut,
            ReasonOrder, quantity, reference, cancellationToken);
        if (!staged)
        {
            throw new OrdersReplenishmentException(OrdersReplenishmentError.InsufficientStock());
        }
    }

    public async Task RegisterEntryAsync(int userId, int productId, int quantity, StockEntryReason reason,
        string reference, CancellationToken cancellationToken = default)
    {
        var inventoryReason = reason == StockEntryReason.Purchase ? ReasonPurchase : ReasonReturn;
        var staged = await inventoryContextFacade.StageStockMovementAsync(userId, productId, MovementIn,
            inventoryReason, quantity, reference, cancellationToken);
        if (!staged)
        {
            throw new OrdersReplenishmentException(OrdersReplenishmentError.ProductRequired());
        }
    }
}
