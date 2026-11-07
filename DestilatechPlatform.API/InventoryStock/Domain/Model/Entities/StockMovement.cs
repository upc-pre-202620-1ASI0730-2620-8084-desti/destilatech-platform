namespace DestilatechPlatform.API.InventoryStock.Domain.Model.Entities;

public sealed class StockMovement
{
    public int Id { get; private set; }
    public int StockItemId { get; private set; }
    public int ProductId { get; private set; }
    public int UserId { get; private set; }
    public string Type { get; private set; } = "";
    public string Reason { get; private set; } = "";
    public decimal Quantity { get; private set; }
    public string Notes { get; private set; } = "";
    public string Reference { get; private set; } = "";
    public DateTime MovementDate { get; private set; }

    private StockMovement() { }

    public StockMovement(int stockItemId, int productId, int userId, string type,
        string reason, decimal quantity, string? notes, string? reference)
    {
        if (type is not ("IN" or "OUT"))
            throw new ArgumentException("Invalid movement type.");

        if (reason is not ("PURCHASE" or "PRODUCTION" or "SALE" or "ORDER" or
            "RETURN" or "ADJUSTMENT" or "WASTE" or "INITIAL"))
            throw new ArgumentException("Invalid movement reason.");

        if (quantity <= 0 || quantity >= 1_000_000_000m || decimal.Round(quantity, 3) != quantity)
            throw new ArgumentException("Quantity must be positive and have at most three decimals.");

        if (notes?.Length > 255 || reference?.Length > 80)
            throw new ArgumentException("Notes or reference exceeds its maximum length.");

        StockItemId = stockItemId;
        ProductId = productId;
        UserId = userId;
        Type = type;
        Reason = reason;
        Quantity = quantity;
        Notes = notes ?? "";
        Reference = reference ?? "";
        MovementDate = DateTime.UtcNow;
    }
}