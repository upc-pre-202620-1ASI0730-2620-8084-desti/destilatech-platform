using DestilatechPlatform.API.AlertsNotifications.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Shared.Domain.Model.Events;

namespace DestilatechPlatform.API.AlertsNotifications.Domain.Model.Events;

public record AlertAttendedEvent(
    AlertId AlertId,
    AccountId AccountId,
    AlertType Type,
    SourceId SourceId) : IEvent;