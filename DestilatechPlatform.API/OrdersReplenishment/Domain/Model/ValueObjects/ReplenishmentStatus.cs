namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.ValueObjects;

/// <summary>
/// Lifecycle of a replenishment order placed with a supplier.
/// Only a requested order can be received or cancelled.
/// </summary>
public enum ReplenishmentStatus
{
    Requested,
    Received,
    Cancelled
}
