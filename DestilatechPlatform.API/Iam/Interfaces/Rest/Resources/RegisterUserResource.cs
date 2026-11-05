namespace DestilatechPlatform.API.Iam.Interfaces.Rest.Resources;

public record RegisterUserResource(
    string Name,
    string Email,
    string BusinessName,
    string UserType,
    string? PreferredPlan
    );