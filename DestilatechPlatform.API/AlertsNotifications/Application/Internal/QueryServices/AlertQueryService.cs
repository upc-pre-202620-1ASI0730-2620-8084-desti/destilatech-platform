using DestilatechPlatform.API.AlertsNotifications.Application.Services;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Aggregates;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Queries;
using DestilatechPlatform.API.AlertsNotifications.Domain.Repositories;

namespace DestilatechPlatform.API.AlertsNotifications.Application.Internal.QueryServices;

public class AlertQueryService(IAlertRepository alertRepository) : IAlertQueryService
{
    public async Task<IEnumerable<Alert>> Handle(GetAllAlertsByAccountIdQuery query,
        CancellationToken cancellationToken)
    {
        return await alertRepository.FindByAccountIdAsync(query.AccountId, query.Type, query.Status,
            cancellationToken);
    }

    public async Task<Alert?> Handle(GetAlertByIdQuery query, CancellationToken cancellationToken)
    {
        return await alertRepository.FindByIdAsync(query.Id, cancellationToken);
    }
}