namespace DestilatechPlatform.API.AnalyticsEstimations.Interfaces.Rest.Resources;

/// <summary>
/// Units of a product that left stock in the requested period.
/// </summary>
public record ProductConsumptionResource(int ProductId, decimal Units);
