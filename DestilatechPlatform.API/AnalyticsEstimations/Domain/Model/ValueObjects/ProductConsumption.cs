namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.ValueObjects;

/// <summary>
/// Units of a product that left stock in a period, used to rank the most consumed products.
/// </summary>
public readonly record struct ProductConsumption(int ProductId, decimal Units);
