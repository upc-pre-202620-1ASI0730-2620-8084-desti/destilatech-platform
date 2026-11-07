using DestilatechPlatform.API.Shared.Domain.Model.Entities;

namespace DestilatechPlatform.API.AnalyticsEstimations.Domain.Model.Aggregates;

/// <summary>
/// Audit timestamps of <see cref="ReplenishmentEstimate"/>, set automatically when it is saved.
/// </summary>
public partial class ReplenishmentEstimate : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
