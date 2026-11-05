namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model;

/// <summary>
/// Business error of the Analytics &amp; Estimations bounded context.
/// </summary>
/// <param name="Code">Stable error code, in kebab case, that the REST layer can map to an HTTP status.</param>
/// <param name="Message">English description of the error.</param>
public sealed record AnalyticsEstimationsError(string Code, string Message)
{
    public static AnalyticsEstimationsError InvalidData(string message) =>
        new("invalid-data", message);

    public static AnalyticsEstimationsError InvalidPeriod(int minDays, int maxDays) =>
        new("invalid-period", $"The period must be between {minDays} and {maxDays} days.");

    public static AnalyticsEstimationsError ProductNotFound(int productId) =>
        new("product-not-found", $"Product {productId} was not found in the inventory of the account.");
}
