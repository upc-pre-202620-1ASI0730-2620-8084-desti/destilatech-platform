namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Queries;

/// <summary>
/// Ranks the products with the most units out of stock in the last days.
/// </summary>
/// <param name="UserId">Account whose history is read.</param>
/// <param name="Days">Length of the period, from 1 to 365 days.</param>
/// <param name="Limit">Maximum number of products to return, from 1 to 50.</param>
public record GetTopConsumedProductsQuery(int UserId, int Days, int Limit = 5);
