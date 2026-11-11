using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Aggregates;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Queries;

namespace DestilatechPlatform.API.AlertsNotifications.Application.Services;

public interface IAlertQueryService
{

    Task<IEnumerable<Alert>> Handle(GetAllAlertsByAccountIdQuery query, CancellationToken cancellationToken);

    Task<Alert?> Handle(GetAlertByIdQuery query, CancellationToken cancellationToken);
}