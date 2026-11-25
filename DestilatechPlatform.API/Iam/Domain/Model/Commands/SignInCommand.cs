namespace DestilatechPlatform.API.Iam.Domain.Model.Commands;

/// <summary>
/// Command to authenticate an account with its email and password.
/// </summary>
public record SignInCommand(string Email, string Password);
