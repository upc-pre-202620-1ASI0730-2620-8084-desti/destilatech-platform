using DestilatechPlatform.API.Iam.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Shared.Domain.Model;

namespace DestilatechPlatform.API.Iam.Domain.Model.Aggregates;

public class User : IAuditableEntity
{
    public const int TrialDays = 14;

    // EF Core necesita un constructor sin parámetros
    protected User()
    {
        FullName = string.Empty;
        Email = string.Empty;
        BusinessName = string.Empty;
    }

    public User(string fullName, string email, string businessName, EBusinessType businessType, string? preferredPlanCode)
    {
        var now = DateTimeOffset.UtcNow;
        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        BusinessName = businessName.Trim();
        BusinessType = businessType;
        PreferredPlanCode = string.IsNullOrWhiteSpace(preferredPlanCode) ? null : preferredPlanCode.Trim();
        TrialStartedAt = now;
        TrialEndsAt = now.AddDays(TrialDays);
    }

    public int Id { get; private set; }
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string BusinessName { get; private set; }
    public EBusinessType BusinessType { get; private set; }
    public string? PreferredPlanCode { get; private set; }
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