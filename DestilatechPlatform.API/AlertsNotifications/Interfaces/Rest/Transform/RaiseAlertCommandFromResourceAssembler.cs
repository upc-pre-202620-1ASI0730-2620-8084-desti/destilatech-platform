using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Commands;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.ValueObjects;
using DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Transform;


public static class RaiseAlertCommandFromResourceAssembler
{

    public static RaiseAlertCommand ToCommandFromResource(RaiseAlertResource resource)
    {
        var severity = string.IsNullOrWhiteSpace(resource.Severity)
            ? AlertSeverity.Warning
            : AlertValueAssembler.ToEnumFromValue<AlertSeverity>(resource.Severity);

        return new RaiseAlertCommand(
            new AccountId(resource.AccountId),
            AlertValueAssembler.ToEnumFromValue<AlertType>(resource.Type),
            new SourceId(resource.SourceId),
            new AlertMessage(resource.Message),
            severity);
    }
}