using DestilatechPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Components;

namespace DestilatechPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Extensions;

public static class RequestAuthorizationMiddlewareExtensions
{
    /// <summary>
    /// Adds the <see cref="RequestAuthorizationMiddleware"/> to the request pipeline.
    /// </summary>
    public static IApplicationBuilder UseRequestAuthorization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestAuthorizationMiddleware>();
    }
}
