using System.Security.Claims;
using System.Text;
using DestilatechPlatform.API.Iam.Application.Internal.OutboundServices;
using DestilatechPlatform.API.Iam.Domain.Model.Aggregates;
using DestilatechPlatform.API.Iam.Infrastructure.Tokens.Jwt.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace DestilatechPlatform.API.Iam.Infrastructure.Tokens.Jwt.Services;

/// <summary>
/// Issues and validates HS256 JSON Web Tokens.
/// </summary>
/// <remarks>
/// Based on the token service of the course reference (learning-center-platform, ADR-008), with three changes:
/// the issuer and the audience are validated, the token expires after <see cref="TokenSettings.ExpirationMinutes"/>,
/// and the secret comes from the environment instead of the appsettings files.
/// </remarks>
public class TokenService(IOptions<TokenSettings> tokenSettings, ILogger<TokenService> logger) : ITokenService
{
    private readonly TokenSettings _settings = tokenSettings.Value;

    public AccessToken GenerateToken(User user)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_settings.ExpirationMinutes);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.BusinessType.ToString().ToUpperInvariant())
            ]),
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = new SigningCredentials(SigningKey(), SecurityAlgorithms.HmacSha256)
        };
        var token = new JsonWebTokenHandler().CreateToken(tokenDescriptor);
        return new AccessToken(token, expiresAt);
    }

    public async Task<int?> ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;

        var result = await new JsonWebTokenHandler().ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SigningKey(),
            ValidateIssuer = true,
            ValidIssuer = _settings.Issuer,
            ValidateAudience = true,
            ValidAudience = _settings.Audience,
            ValidateLifetime = true,
            // Expiration without delay
            ClockSkew = TimeSpan.Zero
        });
        if (!result.IsValid)
        {
            logger.LogDebug("Rejected access token: {Reason}", result.Exception?.Message);
            return null;
        }

        var jwtToken = (JsonWebToken)result.SecurityToken;
        var sid = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;
        return int.TryParse(sid, out var userId) ? userId : null;
    }

    private SymmetricSecurityKey SigningKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
    }
}
