using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;

namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Services;

/// <summary>
/// Domain service that groups the history of an account into periods for the indicators charts (US21).
/// It follows the same rules as the web application: daily periods up to 31 days, weekly periods
/// (starting on Monday) up to 120 days, and monthly periods for longer ranges.
/// </summary>
public static class HistoricalIndicatorCalculator
{
    private const int MaxDaysForDailyPeriods = 31;
    private const int MaxDaysForWeeklyPeriods = 120;
    private const int DaysPerWeek = 7;

    public static Granularity GranularityFor(int days)
    {
        return days switch
        {
            <= MaxDaysForDailyPeriods => Granularity.Day,
            <= MaxDaysForWeeklyPeriods => Granularity.Week,
            _ => Granularity.Month
        };
    }

    /// <summary>
    /// Start of the period that contains <paramref name="date"/>, in the given time zone offset.
    /// </summary>
    public static DateTimeOffset PeriodStart(DateTimeOffset date, Granularity granularity, TimeSpan utcOffset)
    {
        var local = date.ToOffset(utcOffset);
        var start = new DateTimeOffset(local.Year, local.Month, granularity == Granularity.Month ? 1 : local.Day,
            0, 0, 0, utcOffset);
        if (granularity == Granularity.Week)
        {
            var daysSinceMonday = ((int)start.DayOfWeek + DaysPerWeek - 1) % DaysPerWeek;
            start = start.AddDays(-daysSinceMonday);
        }

        return start;
    }

    /// <summary>
    /// Starts of every period between <paramref name="days"/> ago and <paramref name="now"/>, oldest first.
    /// </summary>
    public static IReadOnlyList<DateTimeOffset> BuildPeriods(int days, Granularity granularity, DateTimeOffset now,
        TimeSpan utcOffset)
    {
        var periods = new List<DateTimeOffset>();
        var cursor = PeriodStart(now.AddDays(-days), granularity, utcOffset);
        var last = PeriodStart(now, granularity, utcOffset);
        while (cursor <= last)
        {
            periods.Add(cursor);
            cursor = granularity switch
            {
                Granularity.Month => cursor.AddMonths(1),
                Granularity.Week => cursor.AddDays(DaysPerWeek),
                _ => cursor.AddDays(1)
            };
        }

        return periods;
    }

    /// <summary>
    /// Adds up the values that fall in each period. Values outside the periods are ignored.
    /// </summary>
    public static IReadOnlyList<HistoricalIndicator> SumSeries(MetricType metricType,
        IEnumerable<(DateTimeOffset Date, decimal Value)> records, IReadOnlyList<DateTimeOffset> periods,
        Granularity granularity, TimeSpan utcOffset)
    {
        var totals = periods.ToDictionary(period => period.UtcTicks, _ => 0m);
        foreach (var record in records)
        {
            var key = PeriodStart(record.Date, granularity, utcOffset).UtcTicks;
            if (totals.ContainsKey(key))
            {
                totals[key] += record.Value;
            }
        }

        return periods
            .Select(period => new HistoricalIndicator(metricType, period, totals[period.UtcTicks]))
            .ToList();
    }

    /// <summary>
    /// Rebuilds the total stock at the end of each period by undoing, from the current total,
    /// the movements that happened after that period.
    /// </summary>
    public static IReadOnlyList<HistoricalIndicator> StockLevelSeries(int currentTotal,
        IEnumerable<StockMovementRecord> movements, IReadOnlyList<DateTimeOffset> periods)
    {
        var movementList = movements.ToList();
        return periods
            .Select((period, index) =>
            {
                var periodEnd = index < periods.Count - 1 ? periods[index + 1] : DateTimeOffset.MaxValue;
                var laterChange = movementList
                    .Where(movement => movement.Date >= periodEnd)
                    .Sum(movement => movement.SignedQuantity);
                return new HistoricalIndicator(MetricType.StockLevel, period, Math.Max(0, currentTotal - laterChange));
            })
            .ToList();
    }
}
