using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Aggregates;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.ValueObjects;
using DestilatechPlatform.API.AlertsNotifications.Domain.Repositories;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.AlertsNotifications.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class AlertRepository(AppDbContext context) : BaseRepository<Alert>(context), IAlertRepository
{
    public async Task<IEnumerable<Alert>> FindByAccountIdAsync(AccountId accountId, AlertType? type = null,
        AlertStatus? status = null, CancellationToken cancellationToken = default)
    {
        var alerts = Context.Set<Alert>().Where(a => a.AccountId == accountId);
        if (type is not null) alerts = alerts.Where(a => a.Type == type);
        if (status is not null) alerts = alerts.Where(a => a.Status == status);
        return await alerts.OrderByDescending(a => a.Id).ToListAsync(cancellationToken);
    }
    
    public async Task<Alert?> FindPendingByEpisodeKeyAsync(string episodeKey,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Alert>()
            .FirstOrDefaultAsync(a => a.EpisodeKey == episodeKey && a.Status == AlertStatus.Pending,
                cancellationToken);
    }
}