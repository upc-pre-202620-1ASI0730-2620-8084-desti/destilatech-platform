using DestilatechPlatform.API.AlertsNotifications.Domain.Model.ValueObjects;

namespace DestilatechPlatform.API.AlertsNotifications.Domain.Model.Queries;

public record GetAllAlertsByAccountIdQuery(AccountId AccountId, AlertType? Type = null, AlertStatus? Status = null);