using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Aggregates;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Shared.Domain.Repositories;

namespace DestilatechPlatform.API.AlertsNotifications.Domain.Repositories;

public interface IAlertRepository : IBaseRepository<Alert>
{

    Task<IEnumerable<Alert>> FindByAccountIdAsync(AccountId accountId, AlertType? type = null,
        AlertStatus? status = null, CancellationToken cancellationToken = default);
    
    Task<Alert?> FindPendingByEpisodeKeyAsync(string episodeKey, CancellationToken cancellationToken = default);
}