using DestilatechPlatform.API.AlertsNotifications.Application.Errors;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Aggregates;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Commands;
using DestilatechPlatform.API.Shared.Application.Patterns;

namespace DestilatechPlatform.API.AlertsNotifications.Application.Services;

public interface IAlertCommandService
{
    Task<Result<Alert, ERaiseAlertError>> Handle(RaiseAlertCommand command, CancellationToken cancellationToken);
    
    Task<Result<Alert, EMarkAlertAsAttendedError>> Handle(MarkAlertAsAttendedCommand command,
        CancellationToken cancellationToken);
}