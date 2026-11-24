namespace DestilatechPlatform.API.Iam.Infrastructure.Tokens.Jwt.Configuration;

/// <summary>
/// Settings of the JSON Web Tokens, read from the <c>TokenSettings</c> configuration section.
/// </summary>
/// <remarks>
/// The secret is never stored in the repository: it comes from the <c>TokenSettings__Secret</c> environment
/// variable (Render) or from the user secrets of the project (local development).
/// </remarks>
public class TokenSettings
{
    public const string SectionName = "TokenSettings";

    /// <summary>HS256 needs a key of at least 256 bits, that is 32 bytes.</summary>
    public const int MinimumSecretBytes = 32;

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}
