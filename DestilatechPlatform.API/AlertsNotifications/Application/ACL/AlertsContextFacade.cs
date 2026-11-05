using DestilatechPlatform.API.AlertsNotifications.Application.Services;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Commands;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.ValueObjects;
using DestilatechPlatform.API.AlertsNotifications.Interfaces.ACL;

namespace DestilatechPlatform.API.AlertsNotifications.Application.ACL;


public class AlertsContextFacade(IAlertCommandService alertCommandService) : IAlertsContextFacade
{
    public Task<int> RaiseLowStockAlert(int accountId, int productId, string message,
        CancellationToken cancellationToken = default) =>
        RaiseAlert(accountId, AlertType.LowStock, productId, message, AlertSeverity.Warning, cancellationToken);
    
    public Task<int> RaiseAnomalyAlert(int accountId, int processVariableId, string message,
        bool isCritical = false, CancellationToken cancellationToken = default) =>
        RaiseAlert(accountId, AlertType.Anomaly, processVariableId, message,
            isCritical ? AlertSeverity.Critical : AlertSeverity.Warning, cancellationToken);

    private async Task<int> RaiseAlert(int accountId, AlertType type, int sourceId, string message,
        AlertSeverity severity, CancellationToken cancellationToken)
    {
        var command = new RaiseAlertCommand(new AccountId(accountId), type, new SourceId(sourceId),
            new AlertMessage(message), severity);
        var result = await alertCommandService.Handle(command, cancellationToken);
        return result.IsSuccess ? result.Value!.Id.Value : 0;
    }
}