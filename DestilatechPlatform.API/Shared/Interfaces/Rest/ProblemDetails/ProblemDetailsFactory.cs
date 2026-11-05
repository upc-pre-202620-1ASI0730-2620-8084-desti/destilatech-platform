using DestilatechPlatform.API.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DestilatechPlatform.API.Shared.Interfaces.Rest.ProblemDetails;

public class ProblemDetailsFactory(
    IStringLocalizer<SharedResource> localizer,
    Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory aspNetCoreProblemDetailsFactory)
{
    public IActionResult CreateProblemDetails(
        ControllerBase controller,
        int statusCode,
        Enum? errorEnum,
        string detailMessage)
    {
        var title = errorEnum is not null
            ? localizer[errorEnum.ToString()].Value
            : localizer["UnexpectedServerError"].Value;

        var problemDetails = aspNetCoreProblemDetailsFactory.CreateProblemDetails(
            controller.HttpContext,
            statusCode,
            title,
            detail: detailMessage,
            instance: controller.HttpContext.Request.Path);

        return controller.StatusCode(statusCode, problemDetails);
    }
}