namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Commands;

/// <summary>
/// Calculates and stores a new replenishment estimate for every product of the account,
/// for example to refresh the dashboard.
/// </summary>
public record RecalculateReplenishmentEstimatesCommand(int UserId);
