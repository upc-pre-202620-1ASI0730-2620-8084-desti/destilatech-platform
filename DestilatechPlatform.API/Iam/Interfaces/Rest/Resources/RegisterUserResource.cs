namespace DestilatechPlatform.API.Iam.Interfaces.Rest.Resources;

public record RegisterUserResource(
    string FullName,
    string Email,
    string BusinessName,
    string BusinessType,
    string? PreferredPlanCode
    );