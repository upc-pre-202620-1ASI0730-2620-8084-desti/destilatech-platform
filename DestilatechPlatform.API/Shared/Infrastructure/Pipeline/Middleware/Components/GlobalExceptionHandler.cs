using DestilatechPlatform.API.Resources;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DestilatechPlatform.API.Shared.Infrastructure.Pipeline.Middleware.Components;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService,
    IStringLocalizer<SharedResource> localizer)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException operationCanceledException)
        {
            logger.LogWarning(operationCanceledException, "Request was cancelled: {Message}",
                operationCanceledException.Message);
            return await WriteProblemDetailsAsync(httpContext, exception, StatusCodes.Status409Conflict,
                localizer["OperationCancelled"].Value, localizer["OperationCancelled"].Value);
        }

        logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
        return await WriteProblemDetailsAsync(httpContext, exception, StatusCodes.Status500InternalServerError,
            localizer["UnexpectedServerError"].Value, localizer["UnexpectedErrorProcessingRequest"].Value);
    }

    private async ValueTask<bool> WriteProblemDetailsAsync(HttpContext httpContext, Exception exception,
        int statusCode, string title, string detail)
    {
        httpContext.Response.StatusCode = statusCode;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            },
            Exception = exception
        });
    }
}
