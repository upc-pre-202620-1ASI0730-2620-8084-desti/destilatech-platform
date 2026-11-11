namespace DestilatechPlatform.API.Billing.Interfaces.Rest.Resources;

public record SubscriptionResource( int Id,
    int UserId,
    int PlanId,
    string Status,
    DateTimeOffset StartDate,
    DateTimeOffset RenewalDate,
    string PaymentReference);