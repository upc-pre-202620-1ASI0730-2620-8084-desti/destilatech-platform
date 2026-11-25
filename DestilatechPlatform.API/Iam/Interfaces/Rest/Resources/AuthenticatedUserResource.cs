namespace DestilatechPlatform.API.Iam.Interfaces.Rest.Resources;

/// <summary>
/// The signed-in account and its access token, returned by <c>POST /api/v1/tokens</c>.
/// </summary>
public record AuthenticatedUserResource(UserResource User, string Token, DateTimeOffset ExpiresAt);
