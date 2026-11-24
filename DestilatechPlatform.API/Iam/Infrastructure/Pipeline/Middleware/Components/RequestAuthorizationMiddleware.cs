using DestilatechPlatform.API.Iam.Application.Internal.OutboundServices;
using DestilatechPlatform.API.Iam.Application.QueryServices;
using DestilatechPlatform.API.Iam.Domain.Model.Queries;
using DestilatechPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;

namespace DestilatechPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Components;

/// <summary>
/// Reads the bearer token of the request, validates it and stores the signed-in account in
/// <c>HttpContext.Items["User"]</c>, where the <see cref="AuthorizeAttribute"/> looks for it.
/// </summary>
/// <remarks>
/// Same flow as the request authorization middleware of the course reference (learning-center-platform):
/// it never rejects a request by itself, the attribute decides.
/// </remarks>
public class RequestAuthorizationMiddleware(RequestDelegate next, ILogger<RequestAuthorizationMiddleware> logger)
{
    public const string UserItemKey = "User";

    public async Task InvokeAsync(HttpContext context, IUserQueryService userQueryService, ITokenService tokenService)
    {
        var allowAnonymous = context.GetEndpoint()?.Metadata.GetMetadata<AllowAnonymousAttribute>() is not null;
        if (!allowAnonymous)
        {
            var header = context.Request.Headers.Authorization.FirstOrDefault();
            if (header is not null && header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var userId = await tokenService.ValidateToken(header["Bearer ".Length..].Trim());
                if (userId is not null)
                {
                    var user = await userQueryService.Handle(new GetUserByIdQuery(userId.Value), context.RequestAborted);
                    if (user is not null) context.Items[UserItemKey] = user;
                    else logger.LogDebug("The account {UserId} of the access token no longer exists", userId);
                }
            }
        }

        await next(context);
    }
}
