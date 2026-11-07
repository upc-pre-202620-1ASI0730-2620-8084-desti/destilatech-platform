namespace DestilatechPlatform.API.AlertsNotifications.Interfaces.ACL;

public interface IAlertsContextFacade
{
    Task<int> RaiseLowStockAlert(int accountId, int productId, string message,
        CancellationToken cancellationToken = default);
    
    Task<int> RaiseAnomalyAlert(int accountId, int processVariableId, string message, bool isCritical = false,
        CancellationToken cancellationToken = default);
}