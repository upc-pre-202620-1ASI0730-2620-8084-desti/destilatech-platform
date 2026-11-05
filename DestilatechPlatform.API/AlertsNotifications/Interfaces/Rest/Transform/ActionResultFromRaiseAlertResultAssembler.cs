using DestilatechPlatform.API.AlertsNotifications.Application.Errors;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Aggregates;
using DestilatechPlatform.API.Resources;
using DestilatechPlatform.API.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Transform;

public static class ActionResultFromRaiseAlertResultAssembler
{

    public static ActionResult ToActionResultFromRaiseAlertResult(
        Result<Alert, ERaiseAlertError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer,
        string getAlertByIdActionName) =>
        result switch
        {
            Result<Alert, ERaiseAlertError>.Success success =>
                controller.CreatedAtAction(getAlertByIdActionName, new { id = success.Value.Id },
                    AlertResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Alert, ERaiseAlertError>.Failure failure =>
                failure.Error switch
                {
                    ERaiseAlertError.DuplicatePendingAlert =>
                        controller.Conflict(localizer["AlertDuplicated"].Value),

                    _ => controller.Problem(
                        title: localizer["UnexpectedServerError"].Value,
                        detail: localizer["UnexpectedErrorRaisingAlert"].Value,
                        statusCode: 500)
                },

            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: 500)
        };
}
