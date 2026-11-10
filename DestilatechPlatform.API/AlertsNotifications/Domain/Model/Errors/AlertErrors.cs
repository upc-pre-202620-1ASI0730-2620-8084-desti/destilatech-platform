using DestilatechPlatform.API.Shared.Domain.Model.Errors;

namespace DestilatechPlatform.API.AlertsNotifications.Domain.Model.Errors;

public static class AlertErrors
{
    public static readonly Error AlertNotFound =
        Error.NotFound("Alerts.AlertNotFound", "The specified alert was not found.");

    public static readonly Error DuplicatePendingAlert =
        Error.Conflict("Alerts.DuplicatePendingAlert", "A pending alert already exists for this source.");

    public static readonly Error AlertAlreadyAttended =
        Error.Conflict("Alerts.AlertAlreadyAttended", "The alert was already attended.");

    public static readonly Error InvalidAlertData =
        Error.Validation("Alerts.InvalidAlertData", "The alert data provided is invalid.");

    public static readonly Error DatabaseError =
        Error.Failure("Alerts.DatabaseError", "A database error occurred while processing the alert.");

    public static readonly Error UnexpectedError =
        Error.Unexpected("Alerts.UnexpectedError", "An unexpected error occurred while processing the alert.");
}