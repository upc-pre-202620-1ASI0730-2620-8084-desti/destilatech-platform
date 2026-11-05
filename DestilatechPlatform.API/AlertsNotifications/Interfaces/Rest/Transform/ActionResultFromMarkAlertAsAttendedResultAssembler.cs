using DestilatechPlatform.API.AlertsNotifications.Application.Errors;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Aggregates;
using DestilatechPlatform.API.Resources;
using DestilatechPlatform.API.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Transform;


public static class ActionResultFromMarkAlertAsAttendedResultAssembler
{
    public static ActionResult ToActionResultFromMarkAlertAsAttendedResult(
        Result<Alert, EMarkAlertAsAttendedError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<Alert, EMarkAlertAsAttendedError>.Success success =>
                controller.Ok(AlertResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Alert, EMarkAlertAsAttendedError>.Failure failure =>
                failure.Error switch
                {
                    EMarkAlertAsAttendedError.AlertNotFound =>
                        controller.NotFound(localizer["AlertNotFound"].Value),

                    EMarkAlertAsAttendedError.AlertAlreadyAttended =>
                        controller.Conflict(localizer["AlertAlreadyAttended"].Value),

                    _ => controller.Problem(
                        title: localizer["UnexpectedServerError"].Value,
                        detail: localizer["UnexpectedErrorAttendingAlert"].Value,
                        statusCode: 500)
                },

            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: 500)
        };
}
