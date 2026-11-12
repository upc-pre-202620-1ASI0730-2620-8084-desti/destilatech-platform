using DestilatechPlatform.API.Shared.Domain.Model.Entities;

namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;

/// <summary>
/// Audit timestamps of <see cref="ProductionBatch"/>, set automatically when it is saved.
/// </summary>
public sealed partial class ProductionBatch : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
