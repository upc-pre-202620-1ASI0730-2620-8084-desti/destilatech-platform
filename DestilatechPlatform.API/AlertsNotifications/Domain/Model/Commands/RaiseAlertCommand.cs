using DestilatechPlatform.API.AlertsNotifications.Domain.Model.ValueObjects;

namespace DestilatechPlatform.API.AlertsNotifications.Domain.Model.Commands;

public record RaiseAlertCommand(
    AccountId AccountId,
    AlertType Type,
    SourceId SourceId,
    AlertMessage Message,
    AlertSeverity Severity);