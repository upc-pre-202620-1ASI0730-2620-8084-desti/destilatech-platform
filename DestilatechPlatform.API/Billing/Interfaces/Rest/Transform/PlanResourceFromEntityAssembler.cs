using DestilatechPlatform.API.Billing.Domain.Model.Aggregates;
using DestilatechPlatform.API.Billing.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.Billing.Interfaces.Rest.Transform;

public static class PlanResourceFromEntityAssembler
{
    public static PlanResource ToResourceFromEntity(Plan plan)
    {
        return new PlanResource(plan.Id, plan.Code.ToString().ToUpperInvariant(), plan.Price,
            plan.BillingCycle.ToString().ToUpperInvariant(), plan.MaxUsers, plan.Recommended, plan.FeatureKeys);
    }
}