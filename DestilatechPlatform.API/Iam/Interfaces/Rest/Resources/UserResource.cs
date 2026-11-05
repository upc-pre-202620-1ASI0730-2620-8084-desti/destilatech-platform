namespace DestilatechPlatform.API.Iam.Interfaces.Rest.Resources;

public record UserResource(
    int Id,
    string Name,
    string Email,
    string BusinessName,
    string UserType,
    string? PreferredPlan,
    DateTimeOffset TrialStartedAt,
    DateTimeOffset TrialEndsAt,
    DateTimeOffset? AccessGrantedUntil,
    DateTimeOffset? CreatedAt
    );