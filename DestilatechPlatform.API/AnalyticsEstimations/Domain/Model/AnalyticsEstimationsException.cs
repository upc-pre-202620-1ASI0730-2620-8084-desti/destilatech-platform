namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model;

/// <summary>
/// Raised by the domain of Analytics &amp; Estimations when its input breaks a rule.
/// The application services catch it and return its <see cref="Error"/> as a failed result.
/// </summary>
public class AnalyticsEstimationsException(AnalyticsEstimationsError error) : Exception(error.Message)
{
    public AnalyticsEstimationsError Error { get; } = error;
}
