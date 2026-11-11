namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Queries;

/// <summary>
/// Gets the historical indicator series of the account for the last days (US21).
/// </summary>
/// <param name="UserId">Account whose history is read.</param>
/// <param name="Days">Length of the period, from 1 to 365 days. The web app offers 7, 30, 90 and 365.</param>
/// <param name="UtcOffset">Time zone offset used to split the periods. Defaults to Peru time (UTC-05:00).</param>
public record GetHistoricalIndicatorsQuery(int UserId, int Days, TimeSpan? UtcOffset = null);
