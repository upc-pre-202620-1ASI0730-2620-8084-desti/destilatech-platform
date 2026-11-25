namespace DestilatechPlatform.API.Iam.Application.Internal.OutboundServices;

/// <summary>
/// Hashes and verifies passwords, so the IAM context never stores a plain password.
/// </summary>
public interface IHashingService
{
    string HashPassword(string password);

    bool VerifyPassword(string password, string passwordHash);
}
