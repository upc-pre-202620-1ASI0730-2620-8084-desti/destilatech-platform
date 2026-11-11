namespace DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Resources;

/// <summary>
/// Replenishment estimate of a product (US29). When <c>HasEnoughData</c> is false the product had fewer
/// than three exits in the last 30 days, so there are no days, date or suggested quantity.
/// </summary>
/// <param name="CurrentQuantity">Stock of the product when the estimate was calculated.</param>
/// <param name="LowStockThreshold">Low stock threshold of the product when the estimate was calculated.</param>
/// <param name="Urgent">Whether the product reaches its threshold in seven days or less.</param>
public record ReplenishmentEstimateResource(
    int Id,
    int UserId,
    int ProductId,
    bool HasEnoughData,
    decimal AverageDailyConsumption,
    int? DaysUntilThreshold,
    DateTimeOffset? EstimatedDate,
    int SuggestedQuantity,
    decimal Confidence,
    int ExitsAnalyzed,
    decimal CurrentQuantity,
    decimal LowStockThreshold,
    bool Urgent,
    DateTimeOffset CalculatedAt);
