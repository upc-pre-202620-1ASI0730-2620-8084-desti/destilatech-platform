namespace DestilatechPlatform.API.Iam.Domain.Model.Commands;

public record RegisterUserCommand(
    string FullName,
    string Email,
    string Password,
    string BusinessName,
    string BusinessType,
    string? PreferredPlanCode
    );