using DestilatechPlatform.API.Shared.Domain.Model.Aggregates.Internal;

namespace DestilatechPlatform.API.InventoryStock.Domain.Model.Aggregates;

public sealed partial class Product : AggregateRoot
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Name { get; private set; } = "";
    public string Presentation { get; private set; } = "";
    public string Unit { get; private set; } = "";
    public string Category { get; private set; } = "";
    public decimal UnitPrice { get; private set; }
    public string Sku { get; private set; } = "";
    public bool IsActive { get; private set; } = true;

    private Product() { }

    public Product(int userId, string name, string? presentation, string unit,
        string? category, decimal unitPrice, string? sku)
    {
        if (userId <= 0)
            throw new ArgumentOutOfRangeException(nameof(userId));

        UserId = userId;
        Edit(name, presentation, unit, category, unitPrice, sku);
    }

    public void Edit(string name, string? presentation, string unit, string? category,
        decimal unitPrice, string? sku)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 120)
            throw new ArgumentException("Enter a product name of up to 120 characters.");

        if (unit is not ("BOTTLE" or "BOX" or "LITER" or "UNIT"))
            throw new ArgumentException("Invalid measurement unit.");

        if (unitPrice < 0 || unitPrice >= 10_000_000_000m || decimal.Round(unitPrice, 2) != unitPrice)
            throw new ArgumentException("Invalid unit price.");

        var p = (presentation ?? "").Trim();
        var c = (category ?? "").Trim();
        var s = (sku ?? "").Trim();

        if (p.Length > 80 || c.Length > 80 || s.Length > 64)
            throw new ArgumentException("Presentation, category or SKU exceeds its maximum length.");

        Name = name.Trim();
        Presentation = p;
        Unit = unit;
        Category = c;
        UnitPrice = unitPrice;
        Sku = s;
    }

    public void Deactivate() => IsActive = false;
}