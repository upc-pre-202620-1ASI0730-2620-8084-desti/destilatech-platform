using DestilatechPlatform.API.InventoryStock.Domain.Model.Entities;
using DestilatechPlatform.API.Shared.Domain.Model.Aggregates.Internal;

namespace DestilatechPlatform.API.InventoryStock.Domain.Model.Aggregates;

public sealed partial class StockItem : AggregateRoot
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public int UserId { get; private set; }
    public decimal CurrentQuantity { get; private set; }
    public decimal LowStockThreshold { get; private set; }
    public int Version { get; private set; }

    private StockItem() { }

    public StockItem(int productId, int userId, decimal threshold)
    {
        if (productId <= 0 || userId <= 0)
            throw new ArgumentException("Invalid product or account.");

        ProductId = productId;
        UserId = userId;
        ConfigureThreshold(threshold);
    }

    public void ConfigureThreshold(decimal threshold)
    {
        if (threshold < 0 || threshold >= 1_000_000_000m || decimal.Round(threshold, 3) != threshold)
            throw new ArgumentException("Invalid low stock threshold.");

        LowStockThreshold = threshold;
        Version++;
    }

    public StockMovement RegisterMovement(string type, string reason, decimal quantity,
        string? notes = null, string? reference = null)
    {
        var movement = new StockMovement(Id, ProductId, UserId, type, reason, quantity, notes, reference);
        var updated = CurrentQuantity + (type == "IN" ? quantity : -quantity);

        if (updated < 0)
            throw new InvalidOperationException("Insufficient stock.");
        if (updated >= 1_000_000_000m)
            throw new InvalidOperationException("Maximum stock exceeded.");

        CurrentQuantity = updated;
        Version++;
        return movement;
    }

    public bool IsBelowThreshold() => LowStockThreshold > 0 && CurrentQuantity <= LowStockThreshold;
}