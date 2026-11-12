using DestilatechPlatform.API.Shared.Domain.Model.Entities;

namespace DestilatechPlatform.API.Billing.Domain.Model.Aggregates;

/// <summary>
/// Audit timestamps of <see cref="Subscription"/>, set automatically when it is saved.
/// </summary>
public partial class Subscription : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
