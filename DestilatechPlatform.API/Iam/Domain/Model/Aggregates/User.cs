using DestilatechPlatform.API.Iam.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Shared.Domain.Model.Aggregates.Internal;

namespace DestilatechPlatform.API.Iam.Domain.Model.Aggregates;

public partial class User : AggregateRoot
{
    public const int TrialDays = 14;

    // EF Core necesita un constructor sin parámetros
    protected User()
    {
        FullName = string.Empty;
        Email = string.Empty;
        BusinessName = string.Empty;
        PasswordHash = string.Empty;
    }

    /// <summary>
    /// Registers a new account. The 14-day free trial starts at the moment of the registration.
    /// </summary>
    /// <param name="passwordHash">The BCrypt hash of the password; the plain password is never stored.</param>
    public User(string fullName, string email, string passwordHash, string businessName, EBusinessType businessType,
        string? preferredPlanCode)
    {
        var now = DateTimeOffset.UtcNow;
        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        BusinessName = businessName.Trim();
        BusinessType = businessType;
        PreferredPlanCode = string.IsNullOrWhiteSpace(preferredPlanCode) ? null : preferredPlanCode.Trim().ToUpperInvariant();
        TrialStartedAt = now;
        TrialEndsAt = now.AddDays(TrialDays);
    }
    public static User CreateSeed(string fullName, string email, string businessName, EBusinessType businessType,
        string? preferredPlanCode, DateTimeOffset trialStartedAt, DateTimeOffset trialEndsAt,
        DateTimeOffset? accessGrantedUntil)
    {
        var user = new User(fullName, email, string.Empty, businessName, businessType, preferredPlanCode)
        {
            TrialStartedAt = trialStartedAt,
            TrialEndsAt = trialEndsAt,
            AccessGrantedUntil = accessGrantedUntil
        };
        return user;
    }
    
    public int Id { get; private set; }
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string BusinessName { get; private set; }
    public EBusinessType BusinessType { get; private set; }
    public string? PreferredPlanCode { get; private set; }
    public DateTimeOffset TrialStartedAt { get; private set; }
    public DateTimeOffset TrialEndsAt { get; private set; }
    public DateTimeOffset? AccessGrantedUntil { get; private set; }

    // Los llena el AuditableEntityInterceptor de tu Shared

    public bool HasActiveAccess(DateTimeOffset now)
    {
        return now < TrialEndsAt || (AccessGrantedUntil.HasValue && now < AccessGrantedUntil.Value);
    }

    public void ExtendAccess(DateTimeOffset until)
    {
        if (!AccessGrantedUntil.HasValue || until > AccessGrantedUntil.Value) AccessGrantedUntil = until;
    }

    /// <summary>
    /// Days left in the free trial, counting a started day as a full day; 0 when the trial has ended.
    /// </summary>
    public int TrialDaysRemaining(DateTimeOffset now)
    {
        if (now >= TrialEndsAt) return 0;
        return (int)Math.Ceiling((TrialEndsAt - now).TotalDays);
    }

    public bool HasPassword => !string.IsNullOrEmpty(PasswordHash);

    /// <summary>
    /// Sets the password hash of an account that has none yet, such as the demo accounts created before IAM had passwords.
    /// </summary>
    public void AssignPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("The password hash is required.", nameof(passwordHash));
        PasswordHash = passwordHash;
    }
}