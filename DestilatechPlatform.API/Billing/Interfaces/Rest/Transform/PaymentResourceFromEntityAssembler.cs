using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using DestilatechPlatform.API.Billing.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.Billing.Interfaces.Rest.Transform;

public static class PaymentResourceFromEntityAssembler
{
    public static PaymentResource ToResourceFromEntity(Payment payment)
    {
        return new PaymentResource(payment.Id, payment.UserId, payment.PlanId, payment.Amount, payment.Currency,
            payment.Provider.ToString().ToUpperInvariant(), payment.Reference,
            payment.Status.ToString().ToUpperInvariant(), payment.CardBrand, payment.CardLast4, payment.PaidAt);
    }
}