namespace DestilatechPlatform.API.AlertsNotifications.Domain.Model;

public enum AlertsError
{
    None,
    AlertNotFound,
    DuplicatePendingAlert,
    AlertAlreadyAttended,
    InvalidAlertData,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}