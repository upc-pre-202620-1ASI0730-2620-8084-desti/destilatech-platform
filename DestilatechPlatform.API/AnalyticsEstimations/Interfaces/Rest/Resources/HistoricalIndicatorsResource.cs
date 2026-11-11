namespace DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Resources;

/// <summary>
/// Historical indicators of an account (US30): one series per metric, all with the same periods.
/// </summary>
/// <param name="Granularity">DAY for up to 31 days, WEEK for up to 120 days and MONTH for longer periods.</param>
public record HistoricalIndicatorsResource(
    string Granularity,
    IEnumerable<HistoricalIndicatorResource> StockLevel,
    IEnumerable<HistoricalIndicatorResource> Entries,
    IEnumerable<HistoricalIndicatorResource> Exits,
    IEnumerable<HistoricalIndicatorResource> Sales,
    IEnumerable<HistoricalIndicatorResource> Production);
