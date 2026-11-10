namespace DestilatechPlatform.API.Iam.Interfaces.Rest.Resources;

public record UserResource(
    int Id,
    string FullName,
    string Email,
    string BusinessName,
    string BusinessType,
    DateTimeOffset? CreatedAt,
    TrialPeriodResource TrialPeriod,
    DateTimeOffset? AccessGrantedUntil,
    string? PreferredPlanCode
    );
    