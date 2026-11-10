namespace DestilatechPlatform.API.Iam.Interfaces.Rest.Resources;

public record UserResource(
    int Id,
    string FullName,
    string Email,
    string BusinessName,
    string BusinessType,
    string? PreferredPlanCode,
    DateTimeOffset TrialStartedAt,
    DateTimeOffset TrialEndsAt,
    DateTimeOffset? AccessGrantedUntil,
    DateTimeOffset? CreatedAt
    );