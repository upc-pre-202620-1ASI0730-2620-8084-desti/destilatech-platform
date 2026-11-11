using DestilatechPlatform.API.Shared.Domain.Model.Entities;

namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;

/// <summary>
/// Audit timestamps of <see cref="Customer"/>, set automatically when it is saved.
/// </summary>
public partial class Customer : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
