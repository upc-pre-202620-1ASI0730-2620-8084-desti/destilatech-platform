using DestilatechPlatform.API.Iam.Domain.Model.Aggregates;

namespace DestilatechPlatform.API.Iam.Application.Internal.OutboundServices;

/// <summary>
/// Issues and validates the access tokens of the platform.
/// </summary>
public interface ITokenService
{
    AccessToken GenerateToken(User user);

    /// <returns>The id of the account the token belongs to, or <c>null</c> when the token is not valid.</returns>
    Task<int?> ValidateToken(string token);
}

/// <summary>
/// A signed access token and the moment it expires.
/// </summary>
public record AccessToken(string Value, DateTimeOffset ExpiresAt);
