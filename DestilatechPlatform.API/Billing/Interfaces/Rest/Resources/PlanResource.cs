namespace DestilatechPlatform.API.Billing.Interfaces.Rest.Resources;

public record PlanResource(
    int Id,
    string Code,
    decimal Price,
    string BillingCycle,
    int? MaxUsers,
    bool Recommended,
    IReadOnlyList<string> FeatureKeys);