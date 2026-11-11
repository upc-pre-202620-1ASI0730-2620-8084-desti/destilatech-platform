using DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;
using DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Transform;

public static class HistoricalIndicatorsResourceFromValueAssembler
{
    public static HistoricalIndicatorsResource ToResourceFromValue(HistoricalIndicators value)
    {
        return new HistoricalIndicatorsResource(
            AnalyticsEstimationsValueAssembler.ToValueFromEnum(value.Granularity),
            value.StockLevel.Select(ToResourceFromValue),
            value.Entries.Select(ToResourceFromValue),
            value.Exits.Select(ToResourceFromValue),
            value.Sales.Select(ToResourceFromValue),
            value.Production.Select(ToResourceFromValue));
    }

    private static HistoricalIndicatorResource ToResourceFromValue(HistoricalIndicator indicator)
    {
        return new HistoricalIndicatorResource(
            AnalyticsEstimationsValueAssembler.ToValueFromEnum(indicator.MetricType),
            indicator.PeriodStart,
            indicator.Value);
    }
}
