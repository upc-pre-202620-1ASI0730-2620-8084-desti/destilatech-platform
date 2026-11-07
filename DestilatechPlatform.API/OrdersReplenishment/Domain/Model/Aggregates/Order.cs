using System.Globalization;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Entities;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.ValueObjects;

namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;

/// <summary>
/// Order placed by a customer (US18, US19). It starts as Pending, moves forward one status
/// at a time until Delivered, and can be cancelled while it is still open.
/// </summary>
public partial class Order
{
    private const string CodePrefix = "#";
    private const int LastCodeBeforeFirstOrder = 1000;

    private static readonly OrderStatus[] StatusFlow =
        [OrderStatus.Pending, OrderStatus.Preparing, OrderStatus.Shipped, OrderStatus.Delivered];

    private readonly List<OrderLine> _lines = [];

    public int Id { get; private set; }

    public int UserId { get; private set; }

    /// <summary>Human-readable code shown to the user, for example <c>#1037</c>.</summary>
    public string Code { get; private set; }

    public int CustomerId { get; private set; }

    /// <summary>Snapshot of the customer name when the order was placed.</summary>
    public string CustomerName { get; private set; }

    public DateTimeOffset OrderDate { get; private set; }

    public OrderStatus Status { get; private set; }

    public string Notes { get; private set; }

    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

    /// <summary>
    /// Constructor required by Entity Framework Core.
    /// </summary>
    protected Order()
    {
        Code = string.Empty;
        CustomerName = string.Empty;
        Notes = string.Empty;
    }

    public Order(int userId, string code, int customerId, string customerName, IEnumerable<OrderLine> lines,
        string? notes) : this()
    {
        if (userId <= 0)
        {
            throw new OrdersReplenishmentException(
                OrdersReplenishmentError.InvalidData("An order must belong to an account."));
        }

        if (customerId <= 0)
        {
            throw new OrdersReplenishmentException(OrdersReplenishmentError.CustomerRequired());
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new OrdersReplenishmentException(OrdersReplenishmentError.InvalidData("The order code is required."));
        }

        _lines.AddRange(lines ?? []);
        if (_lines.Count == 0)
        {
            throw new OrdersReplenishmentException(OrdersReplenishmentError.LinesRequired());
        }

        UserId = userId;
        Code = code.Trim();
        CustomerId = customerId;
        CustomerName = customerName?.Trim() ?? string.Empty;
        OrderDate = DateTimeOffset.UtcNow;
        Status = OrderStatus.Pending;
        Notes = notes?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Builds the code of the next order from the codes the account already used:
    /// the highest number plus one, starting at <c>#1001</c>.
    /// </summary>
    public static string NextCode(IEnumerable<string> existingCodes)
    {
        var lastNumber = existingCodes
            .Select(code => int.TryParse(code.Replace(CodePrefix, string.Empty), NumberStyles.None,
                CultureInfo.InvariantCulture, out var number) ? number : 0)
            .DefaultIfEmpty(0)
            .Max();
        return $"{CodePrefix}{Math.Max(lastNumber, LastCodeBeforeFirstOrder) + 1}";
    }

    /// <summary>
    /// Next status in the flow, or <c>null</c> when the order is delivered or cancelled.
    /// </summary>
    public OrderStatus? NextStatus()
    {
        var index = Array.IndexOf(StatusFlow, Status);
        return index >= 0 && index < StatusFlow.Length - 1 ? StatusFlow[index + 1] : null;
    }

    public void AdvanceStatus()
    {
        Status = NextStatus() ?? throw new OrdersReplenishmentException(OrdersReplenishmentError.OrderClosed());
    }

    public void Cancel()
    {
        if (!IsOpen())
        {
            throw new OrdersReplenishmentException(OrdersReplenishmentError.OrderClosed());
        }

        Status = OrderStatus.Cancelled;
    }

    public bool IsOpen()
    {
        return Status != OrderStatus.Delivered && Status != OrderStatus.Cancelled;
    }

    public decimal Total()
    {
        return _lines.Sum(line => line.Subtotal());
    }

    public int TotalUnits()
    {
        return _lines.Sum(line => line.Quantity);
    }

    public bool BelongsTo(int userId)
    {
        return UserId == userId;
    }
}
