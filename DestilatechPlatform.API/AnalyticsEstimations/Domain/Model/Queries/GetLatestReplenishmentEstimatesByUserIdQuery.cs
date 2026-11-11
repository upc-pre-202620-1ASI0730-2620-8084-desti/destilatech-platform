namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Queries;

/// <summary>
/// Gets the most recent replenishment estimate of every product of the account.
/// </summary>
public record GetLatestReplenishmentEstimatesByUserIdQuery(int UserId);
