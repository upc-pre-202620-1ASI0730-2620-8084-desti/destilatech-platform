using DestilatechPlatform.API.ProductionMonitoring.Domain.Model;
using DestilatechPlatform.API.Shared.Domain.Model.Errors;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Transform;

/// <summary>
/// Turns a business error into an RFC 7807 problem response, with the error code in the <c>code</c> extension.
/// </summary>
public static class ActionResultFromProductionMonitoringErrorAssembler
{
    public static ActionResult ToActionResultFromError(Error error, ControllerBase controller)
    {
        // A closed batch or an invalid stage transition is a valid request that the batch rules reject.
        var statusCode = error.Code is ProductionMonitoringError.BatchClosedCode
            or ProductionMonitoringError.InvalidStageTransitionCode
            ? StatusCodes.Status422UnprocessableEntity
            : error.Type switch
            {
                EErrorType.NotFound => StatusCodes.Status404NotFound,
                EErrorType.Conflict => StatusCodes.Status409Conflict,
                EErrorType.Validation => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
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
