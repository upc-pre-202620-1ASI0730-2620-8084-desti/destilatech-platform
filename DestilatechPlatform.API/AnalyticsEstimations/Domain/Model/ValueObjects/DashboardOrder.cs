namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;

/// <summary>
/// Recent order shown in the retailer dashboard.
/// </summary>
public record DashboardOrder(
    int OrderId,
    string Code,
    string CustomerName,
    DateTimeOffset OrderDate,
    decimal Total,
    string Status);
