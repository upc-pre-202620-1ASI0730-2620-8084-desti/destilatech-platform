namespace DestilatechPlatform.API.Billing.Interfaces.Rest.Resources;

public record CheckoutConfirmationResource(PaymentResource Payment, SubscriptionResource Subscription);