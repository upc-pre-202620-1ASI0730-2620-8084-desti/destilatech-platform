namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;

/// <summary>
/// Active production batch shown in the producer dashboard.
/// </summary>
public record DashboardBatch(int BatchId, string Code, string Variety, string Stage, decimal EstimatedQuantity);
