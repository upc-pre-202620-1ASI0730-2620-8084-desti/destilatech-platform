using DestilatechPlatform.API.Shared.Domain.Model.Entities;

namespace DestilatechPlatform.API.InventoryStock.Domain.Model.Entities;

/// <summary>
/// Audit timestamps of <see cref="StockMovement"/>, set automatically when it is saved.
/// </summary>
public sealed partial class StockMovement : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
