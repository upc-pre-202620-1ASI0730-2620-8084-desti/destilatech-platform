namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.ValueObjects;

/// <summary>
/// Lifecycle of a customer order. An order moves forward one step at a time
/// (Pending, Preparing, Shipped, Delivered) and can be cancelled while it is still open.
/// </summary>
public enum OrderStatus
{
    Pending,
    Preparing,
    Shipped,
    Delivered,
    Cancelled
}
