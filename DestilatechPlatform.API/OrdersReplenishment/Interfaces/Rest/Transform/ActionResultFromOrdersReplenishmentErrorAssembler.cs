using DestilatechPlatform.API.OrdersReplenishment.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Transform;

/// <summary>
/// Turns a business error into an RFC 7807 problem response. The error code goes in the <c>code</c>
/// extension, so the web app can show its own translated message (orders.errors.&lt;code&gt;).
/// </summary>
public static class ActionResultFromOrdersReplenishmentErrorAssembler
{
    public static ActionResult ToActionResultFromError(OrdersReplenishmentError error, ControllerBase controller)
    {
        var statusCode = error.Code switch
        {
            "customer-not-found" or "order-not-found" or "replenishment-not-found" =>
                StatusCodes.Status404NotFound,
            "insufficient-stock" or "order-closed" or "replenishment-closed" or "invalid-status-transition" =>
                StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };
        var problemDetails = controller.ProblemDetailsFactory.CreateProblemDetails(
            controller.HttpContext,
            statusCode,
            title: error.Message,
            detail: error.Message,
            instance: controller.HttpContext.Request.Path);
        problemDetails.Extensions["code"] = error.Code;
        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode,
            ContentTypes = { "application/problem+json" }
        };
    }
}
