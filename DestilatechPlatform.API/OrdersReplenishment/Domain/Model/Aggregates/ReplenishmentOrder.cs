using System.Globalization;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.ValueObjects;

namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;

/// <summary>
/// Order placed with a supplier to replenish a product. When it is received,
/// its units are added to stock.
/// </summary>
public partial class ReplenishmentOrder
{
    private const string CodePrefix = "RP-";

    public int Id { get; private set; }

    public int UserId { get; private set; }

    /// <summary>Human-readable code shown to the user, for example <c>RP-0003</c>.</summary>
    public string Code { get; private set; }

    public string SupplierName { get; private set; }

    public string SupplierContact { get; private set; }

    public int ProductId { get; private set; }

    /// <summary>Snapshot of the product name when the replenishment was requested.</summary>
    public string ProductName { get; private set; }

    public int Quantity { get; private set; }

    public DateTimeOffset OrderDate { get; private set; }

    public DateTimeOffset? ExpectedDate { get; private set; }

    public DateTimeOffset? ReceivedDate { get; private set; }

    public ReplenishmentStatus Status { get; private set; }

    public string Notes { get; private set; }

    /// <summary>
    /// Constructor required by Entity Framework Core.
    /// </summary>
    protected ReplenishmentOrder()
    {
        Code = string.Empty;
        SupplierName = string.Empty;
        SupplierContact = string.Empty;
        ProductName = string.Empty;
        Notes = string.Empty;
    }

    public ReplenishmentOrder(RequestReplenishmentOrderCommand command, string code, string productName) : this()
    {
        if (command.UserId <= 0)
        {
            throw new OrdersReplenishmentException(
                OrdersReplenishmentError.InvalidData("A replenishment order must belong to an account."));
        }

        if (string.IsNullOrWhiteSpace(command.SupplierName))
        {
            throw new OrdersReplenishmentException(OrdersReplenishmentError.SupplierRequired());
        }

        if (command.ProductId <= 0)
        {
            throw new OrdersReplenishmentException(OrdersReplenishmentError.ProductRequired());
        }

        if (command.Quantity <= 0)
        {
            throw new OrdersReplenishmentException(OrdersReplenishmentError.QuantityRequired());
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new OrdersReplenishmentException(
                OrdersReplenishmentError.InvalidData("The replenishment code is required."));
        }

        UserId = command.UserId;
        Code = code.Trim();
        SupplierName = command.SupplierName.Trim();
        SupplierContact = command.SupplierContact?.Trim() ?? string.Empty;
        ProductId = command.ProductId;
        ProductName = productName?.Trim() ?? string.Empty;
        Quantity = command.Quantity;
        OrderDate = DateTimeOffset.UtcNow;
        ExpectedDate = command.ExpectedDate;
        Status = ReplenishmentStatus.Requested;
        Notes = command.Notes?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Builds the code of a replenishment order from its sequence number, for example <c>RP-0003</c>.
    /// </summary>
    public static string CodeFor(int sequence)
    {
        return $"{CodePrefix}{sequence.ToString("D4", CultureInfo.InvariantCulture)}";
    }

    public void Receive()
    {
        EnsureIsPending();
        Status = ReplenishmentStatus.Received;
        ReceivedDate = DateTimeOffset.UtcNow;
    }

    public void Cancel()
    {
        EnsureIsPending();
        Status = ReplenishmentStatus.Cancelled;
    }

    public bool IsPending()
    {
        return Status == ReplenishmentStatus.Requested;
    }

    public bool BelongsTo(int userId)
    {
        return UserId == userId;
    }

    private void EnsureIsPending()
    {
        if (!IsPending())
        {
            throw new OrdersReplenishmentException(OrdersReplenishmentError.ReplenishmentClosed());
        }
    }
}
