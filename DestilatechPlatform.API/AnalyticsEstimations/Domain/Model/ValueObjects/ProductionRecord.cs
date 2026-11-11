namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;

/// <summary>
/// Production batch read from Production &amp; Monitoring, used for the production volume indicator.
/// </summary>
/// <param name="Date">Batch start date.</param>
/// <param name="Liters">Estimated liters of the batch.</param>
public readonly record struct ProductionRecord(DateTimeOffset Date, decimal Liters);
