using System.Net;
using DestilatechPlatform.API.Billing.Application.Internal.OutboundServices;
using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using DestilatechPlatform.API.Billing.Domain.Model.Errors;
using DestilatechPlatform.API.Shared.Application.Patterns;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Plan = DestilatechPlatform.API.Billing.Domain.Model.Aggregates.Plan;

namespace DestilatechPlatform.API.Billing.Infrastructure.Payments.Stripe;

public class StripePaymentGatewayService(IOptions<StripeOptions> options) : IPaymentGatewayService
{
    private const string Currency = "pen";
    private readonly StripeClient _client = new(options.Value.SecretKey);
    private readonly StripeOptions _options = options.Value;

    public async Task<Result<CheckoutSessionInfo, BillingError>> CreateCheckoutSessionAsync(int userId, Plan plan,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var sessionOptions = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = _options.SuccessUrl,
                CancelUrl = _options.CancelUrl,
                ClientReferenceId = userId.ToString(),
                Metadata = new Dictionary<string, string>
                {
                    ["userId"] = userId.ToString(),
                    ["planId"] = plan.Id.ToString()
                },
                LineItems =
                [
                    new SessionLineItemOptions
                    {
                        Quantity = 1,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = Currency,
                            // Stripe cobra en la unidad menor (céntimos)
                            UnitAmount = (long)Math.Round(plan.Price * 100m),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"Destilatech {plan.Code}"
                            }
                        }
                    }
                ]
            };

            var session = await new SessionService(_client).CreateAsync(sessionOptions,
                cancellationToken: cancellationToken);

            return new Result<CheckoutSessionInfo, BillingError>.Success(
                new CheckoutSessionInfo(session.Id, session.Url));
        }
        catch (StripeException)
        {
            return new Result<CheckoutSessionInfo, BillingError>.Failure(BillingError.PaymentGatewayFailure);
        }
    }

    public async Task<Result<PaidSessionInfo, BillingError>> GetPaidSessionAsync(string sessionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var session = await new SessionService(_client).GetAsync(sessionId,
                new SessionGetOptions { Expand = ["payment_intent.latest_charge"] },
                cancellationToken: cancellationToken);

            if (session.PaymentStatus != "paid")
                return new Result<PaidSessionInfo, BillingError>.Failure(BillingError.PaymentNotCompleted);

            if (!session.Metadata.TryGetValue("userId", out var userIdText) ||
                !session.Metadata.TryGetValue("planId", out var planIdText) ||
                !int.TryParse(userIdText, out var userId) ||
                !int.TryParse(planIdText, out var planId) ||
                session.AmountTotal is null)
                return new Result<PaidSessionInfo, BillingError>.Failure(BillingError.InvalidData);

            var charge = session.PaymentIntent?.LatestCharge;
            var card = charge?.PaymentMethodDetails?.Card;
            var paidAt = charge is not null
                ? new DateTimeOffset(DateTime.SpecifyKind(charge.Created, DateTimeKind.Utc))
                : DateTimeOffset.UtcNow;

            return new Result<PaidSessionInfo, BillingError>.Success(new PaidSessionInfo(
                session.Id,
                userId,
                planId,
                session.AmountTotal.Value / 100m,
                session.Currency,
                card?.Brand,
                card?.Last4,
                paidAt));
        }
        catch (StripeException e) when (e.HttpStatusCode == HttpStatusCode.NotFound)
        {
            return new Result<PaidSessionInfo, BillingError>.Failure(BillingError.SessionNotFound);
        }
        catch (StripeException)
        {
            return new Result<PaidSessionInfo, BillingError>.Failure(BillingError.PaymentGatewayFailure);
        }
    }
}