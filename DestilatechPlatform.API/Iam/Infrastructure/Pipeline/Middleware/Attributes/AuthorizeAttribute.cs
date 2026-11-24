using DestilatechPlatform.API.Iam.Domain.Model.Aggregates;
using DestilatechPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DestilatechPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>
/// Requires a signed-in account. The <see cref="RequestAuthorizationMiddleware"/> validates the access token and
/// stores the account in <c>HttpContext.Items["User"]</c>; when there is no account the request ends with
/// <c>401 Unauthorized</c> as Problem Details.
/// </summary>
/// <remarks>
/// When the route has a <c>tenantId</c>, the account can only reach its own tenant: any other id ends with
/// <c>403 Forbidden</c>. The <c>tenantId</c> sent in a request body is checked by the
/// <see cref="Filters.TenantAccessFilter"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any()) return;

        if (context.HttpContext.Items[RequestAuthorizationMiddleware.UserItemKey] is not User user)
        {
            context.Result = Problem(context, StatusCodes.Status401Unauthorized, "Unauthorized",
                "A valid access token is required. Sign in with POST /api/v1/tokens.");
            return;
        }

        if (context.RouteData.Values.TryGetValue("tenantId", out var routeTenant) &&
            int.TryParse(routeTenant?.ToString(), out var tenantId) && tenantId != user.Id)
            context.Result = Problem(context, StatusCodes.Status403Forbidden, "Forbidden",
                "The access token does not belong to this tenant.");
    }

    internal static ObjectResult Problem(FilterContext context, int status, string title, string detail)
    {
        var factory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
        var problem = factory.CreateProblemDetails(context.HttpContext, status, title, detail: detail);
        return new ObjectResult(problem) { StatusCode = status, ContentTypes = { "application/problem+json" } };
    }
}
