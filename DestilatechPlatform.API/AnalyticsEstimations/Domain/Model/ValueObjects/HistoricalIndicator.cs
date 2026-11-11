namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;

/// <summary>
/// Value of a metric in one period of a historical series (US21).
/// </summary>
public readonly record struct HistoricalIndicator
{
    private const int ValueDecimals = 2;

    public MetricType MetricType { get; }

    public DateTimeOffset PeriodStart { get; }

    public decimal Value { get; }

    public HistoricalIndicator(MetricType metricType, DateTimeOffset periodStart, decimal value)
    {
        MetricType = metricType;
        PeriodStart = periodStart;
        Value = Math.Round(value, ValueDecimals, MidpointRounding.AwayFromZero);
    }
}
