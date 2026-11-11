using DestilatechPlatform.API.Shared.Domain.Model.Entities;

namespace DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;

/// <summary>
/// Audit timestamps of <see cref="Order"/>, set automatically when it is saved.
/// </summary>
public partial class Order : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
