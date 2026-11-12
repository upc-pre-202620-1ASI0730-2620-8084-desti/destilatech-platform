using DestilatechPlatform.API.Shared.Domain.Model.Entities;

namespace DestilatechPlatform.API.Billing.Domain.Model.Aggregates;

/// <summary>
/// Audit timestamps of <see cref="Plan"/>, set automatically when it is saved.
/// </summary>
public partial class Plan : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
