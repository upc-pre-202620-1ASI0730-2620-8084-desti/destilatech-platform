using DestilatechPlatform.API.Shared.Domain.Model.Entities;

namespace DestilatechPlatform.API.AlertsNotifications.Domain.Model.Aggregates;


public partial class Alert : IAuditableEntity
{

    public DateTimeOffset? CreatedAt { get; set; }


    public DateTimeOffset? UpdatedAt { get; set; }
}