using DestilatechPlatform.API.Billing.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Shared.Domain.Model.Aggregates.Internal;

namespace DestilatechPlatform.API.Billing.Domain.Model.Aggregates;

public partial class Payment : AggregateRoot
{
    //Requerido porEF Core
    protected Payment()
    {
        Currency = string.Empty;
        Reference = string.Empty;
    }

    public Payment(int userId, int planId, decimal amount, string currency, EPaymentProvider provider,
        string reference, EPaymentStatus status, string? cardBrand, string? cardLast4, DateTimeOffset paidAt)
    {
        UserId = userId;
        PlanId = planId;
        Amount = amount;
        Currency = currency;
        Provider = provider;
        Reference = reference;
        Status = status;
        CardBrand = cardBrand;
        CardLast4 = cardLast4;
        PaidAt = paidAt;
    }

    public int Id { get; private set; }

    // Referencia lógica a IAM: sin foreign key
    public int UserId { get; private set; }
    public int PlanId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public EPaymentProvider Provider { get; private set; }

    // Id de la checkout session de Stripe (único)
    public string Reference { get; private set; }
    public EPaymentStatus Status { get; private set; }
    public string? CardBrand { get; private set; }
    public string? CardLast4 { get; private set; }
    public DateTimeOffset PaidAt { get; private set; }
}