using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using DestilatechPlatform.API.Billing.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.Billing.Interfaces.Rest.Transform;

public static class SubscriptionResourceFromEntityAssembler
{
    public static SubscriptionResource ToResourceFromEntity(Subscription subscription)
    {
        return new SubscriptionResource(subscription.Id, subscription.UserId, subscription.PlanId,
            subscription.Status.ToString().ToUpperInvariant(), subscription.StartDate, subscription.RenewalDate,
            subscription.PaymentReference);
    }
}