namespace DestilatechPlatform.API.Billing.Infrastructure.Payments.Stripe;

public class StripeOptions
{
    public string SecretKey { get; set; } = string.Empty;

    public string SuccessUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
}
