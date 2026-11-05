using DestilatechPlatform.API.Iam.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Shared.Domain.Model;

namespace DestilatechPlatform.API.Iam.Domain.Model.Aggregates;

public class User : IAuditableEntity
{
    public const int TrialDays = 14;

    // EF Core necesita un constructor sin parámetros
    protected User()
    {
        Name = string.Empty;
        Email = string.Empty;
        BusinessName = string.Empty;
    }

    public User(string name, string email, string businessName, EUserType userType, string? preferredPlan)
    {
        var now = DateTimeOffset.UtcNow;
        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        BusinessName = businessName.Trim();
        UserType = userType;
        PreferredPlan = string.IsNullOrWhiteSpace(preferredPlan) ? null : preferredPlan.Trim();
        TrialStartedAt = now;
        TrialEndsAt = now.AddDays(TrialDays);
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string BusinessName { get; private set; }
    public EUserType UserType { get; private set; }
    public string? PreferredPlan { get; private set; }
    public DateTimeOffset TrialStartedAt { get; private set; }
    public DateTimeOffset TrialEndsAt { get; private set; }
    public DateTimeOffset? AccessGrantedUntil { get; private set; }

    // Los llena el AuditableEntityInterceptor de tu Shared
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public bool HasActiveAccess(DateTimeOffset now)
    {
        return now < TrialEndsAt || (AccessGrantedUntil.HasValue && now < AccessGrantedUntil.Value);
    }

    public void ExtendAccess(DateTimeOffset until)
    {
        if (!AccessGrantedUntil.HasValue || until > AccessGrantedUntil.Value) AccessGrantedUntil = until;
    }
}