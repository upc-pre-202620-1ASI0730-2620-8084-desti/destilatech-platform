using DestilatechPlatform.API.Iam.Application.Internal.OutboundServices;
using BCryptNet = BCrypt.Net.BCrypt;

namespace DestilatechPlatform.API.Iam.Infrastructure.Hashing.BCrypt.Services;

/// <summary>
/// Password hashing with BCrypt, as in the IAM context of the course reference (learning-center-platform).
/// </summary>
public class HashingService : IHashingService
{
    public string HashPassword(string password)
    {
        return BCryptNet.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrEmpty(passwordHash)) return false;
        return BCryptNet.Verify(password, passwordHash);
    }
}
