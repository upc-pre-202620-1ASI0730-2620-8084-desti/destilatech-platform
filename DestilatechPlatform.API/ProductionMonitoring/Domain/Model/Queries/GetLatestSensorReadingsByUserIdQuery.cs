namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Queries;

/// <summary>
/// Lists the latest sensor readings of the account, up to 500.
/// </summary>
public record GetLatestSensorReadingsByUserIdQuery(int UserId, int? Limit);
