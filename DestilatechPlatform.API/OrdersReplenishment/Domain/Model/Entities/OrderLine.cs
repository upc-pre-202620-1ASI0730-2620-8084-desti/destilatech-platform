namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Entities;

/// <summary>
/// Product line of an order. It keeps a snapshot of the product name and unit price,
/// so later changes in the product catalog do not alter past orders.
/// </summary>
public class OrderLine
{
    public int Id { get; private set; }

    public int OrderId { get; private set; }

    public int ProductId { get; private set; }

    public string ProductName { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// Constructor required by Entity Framework Core.
    /// </summary>
    protected OrderLine()
    {
        ProductName = string.Empty;
    }

    public OrderLine(int productId, string productName, int quantity, decimal unitPrice)
    {
        if (productId <= 0)
        {
            throw new OrdersReplenishmentException(OrdersReplenishmentError.ProductRequired());
        }

        if (quantity <= 0)
        {
            throw new OrdersReplenishmentException(OrdersReplenishmentError.QuantityRequired());
        }

        if (unitPrice < 0)
        {
            throw new OrdersReplenishmentException(
                OrdersReplenishmentError.InvalidData("The unit price cannot be negative."));
        }

        ProductId = productId;
        ProductName = productName?.Trim() ?? string.Empty;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public decimal Subtotal()
    {
        return Quantity * UnitPrice;
    }
}
