using DestilatechPlatform.API.Shared.Domain.Model.Entities;

namespace DestilatechPlatform.API.InventoryStock.Domain.Model.Aggregates;

/// <summary>
/// Audit timestamps of <see cref="StockItem"/>, set automatically when it is saved.
/// </summary>
public sealed partial class StockItem : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
