using DestilatechPlatform.API.Iam.Domain.Model.Aggregates;
using DestilatechPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Components;

namespace DestilatechPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Extensions;

public static class HttpContextUserExtensions
{
    /// <returns>The account of the access token, or <c>null</c> when the request has no valid token.</returns>
    public static User? SignedInUser(this HttpContext context)
    {
        return context.Items[RequestAuthorizationMiddleware.UserItemKey] as User;
    }
}
