using DestilatechPlatform.API.Shared.Domain.Model.Entities;

namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;

/// <summary>
/// Audit timestamps of <see cref="ProcessVariable"/>, set automatically when it is saved.
/// </summary>
public sealed partial class ProcessVariable : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
