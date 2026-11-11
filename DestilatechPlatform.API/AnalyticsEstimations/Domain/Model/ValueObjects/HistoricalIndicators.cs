namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;

/// <summary>
/// Every historical series shown in the indicators view for one period (US21).
/// </summary>
/// <param name="Granularity">Size of each period in the series.</param>
/// <param name="StockLevel">Total units in stock at the end of each period.</param>
/// <param name="Entries">Units that entered stock in each period.</param>
/// <param name="Exits">Units that left stock in each period.</param>
/// <param name="Sales">Sales amount of the orders that were not cancelled.</param>
/// <param name="Production">Estimated liters of the batches started in each period.</param>
public record HistoricalIndicators(
    Granularity Granularity,
    IReadOnlyList<HistoricalIndicator> StockLevel,
    IReadOnlyList<HistoricalIndicator> Entries,
    IReadOnlyList<HistoricalIndicator> Exits,
    IReadOnlyList<HistoricalIndicator> Sales,
    IReadOnlyList<HistoricalIndicator> Production);
