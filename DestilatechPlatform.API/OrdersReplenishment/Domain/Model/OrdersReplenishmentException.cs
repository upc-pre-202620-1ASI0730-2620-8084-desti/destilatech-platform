namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model;

/// <summary>
/// Raised by the aggregates of Orders &amp; Replenishment when a business rule is broken.
/// The application services catch it and return its <see cref="Error"/> as a failed result.
/// </summary>
public class OrdersReplenishmentException(OrdersReplenishmentError error) : Exception(error.Message)
{
    public OrdersReplenishmentError Error { get; } = error;
}
