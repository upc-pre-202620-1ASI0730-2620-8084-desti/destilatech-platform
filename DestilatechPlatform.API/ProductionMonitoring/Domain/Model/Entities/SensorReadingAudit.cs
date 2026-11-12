using DestilatechPlatform.API.Shared.Domain.Model.Entities;

namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Entities;

/// <summary>
/// Audit timestamps of <see cref="SensorReading"/>, set automatically when it is saved.
/// </summary>
public sealed partial class SensorReading : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
