using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Aggregates;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Commands;
using DestilatechPlatform.API.Shared.Application.Model;

namespace DestilatechPlatform.API.AlertsNotifications.Application.Services;


public interface IAlertCommandService
{

    Task<Result<Alert>> Handle(RaiseAlertCommand command, CancellationToken cancellationToken);
    
    Task<Result<Alert>> Handle(MarkAlertAsAttendedCommand command, CancellationToken cancellationToken);
}