namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Queries;

/// <summary>
/// Gets the most recent replenishment estimate of a product, shown in its inventory record (US20).
/// </summary>
public record GetLatestReplenishmentEstimateByProductIdQuery(int UserId, int ProductId);
