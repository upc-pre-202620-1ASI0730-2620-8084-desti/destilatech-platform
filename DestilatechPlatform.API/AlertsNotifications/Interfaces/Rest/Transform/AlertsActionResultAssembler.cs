using DestilatechPlatform.API.AlertsNotifications.Domain.Model;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Aggregates;
using DestilatechPlatform.API.Resources;
using DestilatechPlatform.API.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ProblemDetailsFactory = DestilatechPlatform.API.Shared.Interfaces.Rest.ProblemDetails.ProblemDetailsFactory;

namespace DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Transform;

public static class AlertsActionResultAssembler
{

    private static (int StatusCode, string DetailKey) ToHttpErrorFromAlertsError(AlertsError error) => error switch
    {
        AlertsError.AlertNotFound => (StatusCodes.Status404NotFound, "AlertNotFound"),
        AlertsError.DuplicatePendingAlert => (StatusCodes.Status409Conflict, "AlertDuplicated"),
        AlertsError.AlertAlreadyAttended => (StatusCodes.Status409Conflict, "AlertAlreadyAttended"),
        AlertsError.InvalidAlertData => (StatusCodes.Status400BadRequest, "InvalidAlertRequest"),
        AlertsError.OperationCancelled => (StatusCodes.Status409Conflict, "OperationCancelled"),
        _ => (StatusCodes.Status500InternalServerError, "UnexpectedErrorProcessingRequest")
    };

    public static IActionResult ToProblemDetailsFromAlertsError(
        ControllerBase controller,
        AlertsError error,
        IStringLocalizer<SharedResource> localizer,
        ProblemDetailsFactory problemDetailsFactory)
    {
        var (statusCode, detailKey) = ToHttpErrorFromAlertsError(error);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, error, localizer[detailKey].Value);
    }
    
    public static IActionResult ToActionResultFromAlertResult(
        ControllerBase controller,
        Result<Alert> result,
        IStringLocalizer<SharedResource> localizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<Alert, IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction(result.Value!);
        var error = result.Error as AlertsError? ?? AlertsError.InternalServerError;
        return ToProblemDetailsFromAlertsError(controller, error, localizer, problemDetailsFactory);
    }

    public static IActionResult ToActionResultFromGetAlertByIdResult(
        ControllerBase controller,
        Alert? alert,
        IStringLocalizer<SharedResource> localizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<Alert, IActionResult> successAction)
    {
        return alert is null
            ? ToProblemDetailsFromAlertsError(controller, AlertsError.AlertNotFound, localizer, problemDetailsFactory)
            : successAction(alert);
    }
}
