using DestilatechPlatform.API.Billing.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Shared.Domain.Model.Aggregates.Internal;

namespace DestilatechPlatform.API.Billing.Domain.Model.Aggregates;

public partial class Subscription : AggregateRoot
{
    // Requerido por EF Core
    protected Subscription()
    {
        PaymentReference = string.Empty;
    }

    public Subscription(int userId, int planId, string paymentReference, DateTimeOffset startDate)
    {
        UserId = userId;
        PlanId = planId;
        PaymentReference = paymentReference;
        Status = ESubscriptionStatus.Active;
        StartDate = startDate;
        RenewalDate = startDate.AddMonths(1);
    }

    public int Id { get; private set; }

    // Referencia lógica a IAM: sin foreign key
    public int UserId { get; private set; }
    public int PlanId { get; private set; }
    public ESubscriptionStatus Status { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset RenewalDate { get; private set; }

    // Id de la checkout session de Stripe
    public string PaymentReference { get; private set; }

    public void Cancel()
    {
        Status = ESubscriptionStatus.Cancelled;
    }
}