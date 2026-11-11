using DestilatechPlatform.API.Billing.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Shared.Domain.Model.Entities;

namespace DestilatechPlatform.API.Billing.Domain.Model.Aggregates;

public class Plan : IAuditableEntity
{
    // Requerido por EF Core
    protected Plan()
    {
        FeatureKeysCsv = string.Empty;
    }

    public Plan(EPlanCode code, decimal price, EBillingCycle billingCycle, int? maxUsers, bool recommended,
        IEnumerable<string> featureKeys)
    {
        Code = code;
        Price = price;
        BillingCycle = billingCycle;
        MaxUsers = maxUsers;
        Recommended = recommended;
        FeatureKeysCsv = string.Join(',', featureKeys);
    }

    public int Id { get; private set; }
    public EPlanCode Code { get; private set; }
    public decimal Price { get; private set; }
    public EBillingCycle BillingCycle { get; private set; }
    public int? MaxUsers { get; private set; }
    public bool Recommended { get; private set; }

    // Se guarda como texto separado por comas
    public string FeatureKeysCsv { get; private set; }

    // Solo lectura: EF no la mapea porque no tiene setter
    public IReadOnlyList<string> FeatureKeys =>
        FeatureKeysCsv.Split(',', StringSplitOptions.RemoveEmptyEntries);

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}