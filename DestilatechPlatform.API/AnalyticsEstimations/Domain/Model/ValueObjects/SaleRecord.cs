namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;

/// <summary>
/// Order read from Orders &amp; Replenishment, used for the sales volume indicator.
/// </summary>
/// <param name="Date">Order date.</param>
/// <param name="Total">Order total amount.</param>
/// <param name="Cancelled">Whether the order was cancelled. Cancelled orders do not count as sales.</param>
public readonly record struct SaleRecord(DateTimeOffset Date, decimal Total, bool Cancelled);
