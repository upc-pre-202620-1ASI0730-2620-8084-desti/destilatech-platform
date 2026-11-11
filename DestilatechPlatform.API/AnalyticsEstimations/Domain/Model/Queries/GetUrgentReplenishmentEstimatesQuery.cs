namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Queries;

/// <summary>
/// Lists the products that reach their low stock threshold within a week, most urgent first,
/// for the priority replenishment list of the dashboard.
/// </summary>
public record GetUrgentReplenishmentEstimatesQuery(int UserId);
