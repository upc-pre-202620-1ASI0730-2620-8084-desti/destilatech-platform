namespace DestilatechPlatform.API.Billing.Interfaces.Rest.Resources;

public record PaymentResource(
    int Id,
    int UserId,
    int PlanId,
    decimal Amount,
    string Currency,
    string Provider,
    string Reference,
    string Status,
    string? CardBrand,
    string? CardLast4,
    DateTimeOffset PaidAt);