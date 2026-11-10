using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Aggregates;
using DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Resources;

namespace DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Transform;

public static class AlertResourceFromEntityAssembler
{

    public static AlertResource ToResourceFromEntity(Alert entity)
    {
        return new AlertResource(
            entity.Id.Value,
            entity.AccountId.Value,
            AlertValueAssembler.ToValueFromEnum(entity.Type),
            entity.SourceId.Value,
            entity.Message.Value,
            AlertValueAssembler.ToValueFromEnum(entity.Severity),
            AlertValueAssembler.ToValueFromEnum(entity.Status),
            entity.CreatedAt,
            entity.AttendedAt
        );
    }
}