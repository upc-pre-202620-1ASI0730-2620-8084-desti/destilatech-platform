namespace DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Resources;

public record DashboardOrderResource(
    int Id,
    string Code,
    string CustomerName,
    DateTimeOffset OrderDate,
    decimal Total,
    string Status);
