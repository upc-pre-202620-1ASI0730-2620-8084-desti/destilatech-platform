using DestilatechPlatform.API.Shared.Domain.Model.Entities;

namespace DestilatechPlatform.API.Iam.Domain.Model.Aggregates;

/// <summary>
/// Audit timestamps of <see cref="User"/>, set automatically when it is saved.
/// </summary>
public partial class User : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
