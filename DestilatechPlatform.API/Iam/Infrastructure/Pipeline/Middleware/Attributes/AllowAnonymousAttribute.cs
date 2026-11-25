namespace DestilatechPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>
/// Marks an action that does not require an access token, such as the registration or the sign-in.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AllowAnonymousAttribute : Attribute
{
}
