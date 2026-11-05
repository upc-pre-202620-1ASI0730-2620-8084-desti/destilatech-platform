namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;

/// <summary>
/// Current stock of a product and the threshold below which it is considered low.
/// </summary>
public readonly record struct StockLevel(int ProductId, int CurrentQuantity, int LowStockThreshold);
