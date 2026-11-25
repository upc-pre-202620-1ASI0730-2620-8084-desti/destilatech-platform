using DestilatechPlatform.API.Iam.Domain.Model.Aggregates;
using DestilatechPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using DestilatechPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Components;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DestilatechPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Filters;

/// <summary>
/// Checks that the <c>tenantId</c> sent in the body of a create or update request is the signed-in account,
/// so an account cannot write data in another tenant. A different id ends with <c>403 Forbidden</c>.
/// </summary>
/// <remarks>
/// Registered as a global MVC filter. It runs after model binding, when the request resources are available,
/// and only acts when the request has a signed-in account.
/// </remarks>
public class TenantAccessFilter : IAsyncActionFilter
{
    private const string TenantIdProperty = "TenantId";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.Items[RequestAuthorizationMiddleware.UserItemKey] is User user &&
            context.ActionArguments.Values.Any(argument => BelongsToAnotherTenant(argument, user.Id)))
        {
            context.Result = AuthorizeAttribute.Problem(context, StatusCodes.Status403Forbidden, "Forbidden",
                "The tenantId of the request does not belong to the signed-in account.");
            return;
        }

        await next();
    }

    private static bool BelongsToAnotherTenant(object? argument, int userId)
    {
        var property = argument?.GetType().GetProperty(TenantIdProperty);
        return property?.GetValue(argument) is int tenantId && tenantId > 0 && tenantId != userId;
    }
}
