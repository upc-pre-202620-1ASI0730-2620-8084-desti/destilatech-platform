namespace DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Resources;

/// <summary>
/// Value of a metric in one period of a chart, for example the stock exits of a week.
/// </summary>
/// <param name="MetricType">STOCK_LEVEL, STOCK_ENTRIES, STOCK_EXITS, SALES_VOLUME or PRODUCTION_VOLUME.</param>
/// <param name="PeriodStart">Start of the period in the time zone of the request.</param>
/// <param name="Value">Value of the metric, rounded to two decimals.</param>
public record HistoricalIndicatorResource(string MetricType, DateTimeOffset PeriodStart, decimal Value);
