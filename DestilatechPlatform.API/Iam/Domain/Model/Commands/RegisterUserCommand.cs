namespace DestilatechPlatform.API.Iam.Domain.Model.Commands;

public record RegisterUserCommand(
    string Name,
    string Email,
    string BusinessName,
    string UserType,
    string? PreferredPlan
    );