using DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.OutboundServices;
using DestilatechPlatform.API.AnalyticsEstimations.Application.Services;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Queries;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Services;
using DestilatechPlatform.API.Shared.Application.Patterns;

namespace DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.QueryServices;

/// <summary>
/// Builds the historical indicators of an account (US21) from the history of Inventory,
/// Orders and Production, read through their outbound ports.
/// </summary>
public class HistoricalIndicatorQueryService(
    IInventoryHistoryReadPort inventoryHistory,
    ISalesHistoryReadPort salesHistory,
    IProductionHistoryReadPort productionHistory) : IHistoricalIndicatorQueryService
{
    private const int MinDays = 1;
    private const int MaxDays = 365;
    private const int MinLimit = 1;
    private const int MaxLimit = 50;
    private const int MaxOffsetHours = 14;

    /// <summary>Peru time, used when the query does not give a time zone offset.</summary>
    private static readonly TimeSpan DefaultUtcOffset = TimeSpan.FromHours(-5);

    public async Task<Result<HistoricalIndicators, AnalyticsEstimationsError>> Handle(
        GetHistoricalIndicatorsQuery query, CancellationToken cancellationToken = default)
    {
        if (query.Days is < MinDays or > MaxDays)
        {
            return new Result<HistoricalIndicators, AnalyticsEstimationsError>.Failure(
                AnalyticsEstimationsError.InvalidPeriod(MinDays, MaxDays));
        }

        var utcOffset = query.UtcOffset ?? DefaultUtcOffset;
        if (utcOffset.Duration() > TimeSpan.FromHours(MaxOffsetHours) || utcOffset.Seconds != 0)
        {
            return new Result<HistoricalIndicators, AnalyticsEstimationsError>.Failure(
                AnalyticsEstimationsError.InvalidData("The time zone offset must be whole minutes within 14 hours."));
        }

        var now = DateTimeOffset.UtcNow;
        var granularity = HistoricalIndicatorCalculator.GranularityFor(query.Days);
        var periods = HistoricalIndicatorCalculator.BuildPeriods(query.Days, granularity, now, utcOffset);
        var since = periods[0];

        var stockLevels = await inventoryHistory.ReadStockLevelsAsync(query.UserId, cancellationToken);
        var movements = await inventoryHistory.ReadMovementsAsync(query.UserId, since, cancellationToken);
        var sales = await salesHistory.ReadSalesAsync(query.UserId, since, cancellationToken);
        var production = await productionHistory.ReadProductionAsync(query.UserId, since, cancellationToken);

        var entries = movements
            .Where(movement => movement.Direction == MovementDirection.In)
            .Select(movement => (movement.Date, movement.Quantity));
        var exits = movements
            .Where(movement => movement.Direction == MovementDirection.Out)
            .Select(movement => (movement.Date, movement.Quantity));
        var salesAmounts = sales
            .Where(sale => !sale.Cancelled)
            .Select(sale => (sale.Date, sale.Total));
        var liters = production.Select(batch => (batch.Date, batch.Liters));

        var indicators = new HistoricalIndicators(
            granularity,
            HistoricalIndicatorCalculator.StockLevelSeries(stockLevels.Sum(level => level.CurrentQuantity), movements,
                periods),
            HistoricalIndicatorCalculator.SumSeries(MetricType.StockEntries, entries, periods, granularity, utcOffset),
            HistoricalIndicatorCalculator.SumSeries(MetricType.StockExits, exits, periods, granularity, utcOffset),
            HistoricalIndicatorCalculator.SumSeries(MetricType.SalesVolume, salesAmounts, periods, granularity,
                utcOffset),
            HistoricalIndicatorCalculator.SumSeries(MetricType.ProductionVolume, liters, periods, granularity,
                utcOffset));
        return new Result<HistoricalIndicators, AnalyticsEstimationsError>.Success(indicators);
    }

    public async Task<Result<IReadOnlyList<ProductConsumption>, AnalyticsEstimationsError>> Handle(
        GetTopConsumedProductsQuery query, CancellationToken cancellationToken = default)
    {
        if (query.Days is < MinDays or > MaxDays)
        {
            return new Result<IReadOnlyList<ProductConsumption>, AnalyticsEstimationsError>.Failure(
                AnalyticsEstimationsError.InvalidPeriod(MinDays, MaxDays));
        }

        if (query.Limit is < MinLimit or > MaxLimit)
        {
            return new Result<IReadOnlyList<ProductConsumption>, AnalyticsEstimationsError>.Failure(
                AnalyticsEstimationsError.InvalidData($"The limit must be between {MinLimit} and {MaxLimit}."));
        }

        var since = DateTimeOffset.UtcNow.AddDays(-query.Days);
        var movements = await inventoryHistory.ReadMovementsAsync(query.UserId, since, cancellationToken);
        var ranking = movements
            .Where(movement => movement.Direction == MovementDirection.Out && movement.Date >= since)
            .GroupBy(movement => movement.ProductId)
            .Select(group => new ProductConsumption(group.Key, group.Sum(movement => movement.Quantity)))
            .OrderByDescending(consumption => consumption.Units)
            .Take(query.Limit)
            .ToList();
        return new Result<IReadOnlyList<ProductConsumption>, AnalyticsEstimationsError>.Success(ranking);
    }
}
