using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Events;
using DestilatechPlatform.API.Shared.Application.Internal.EventHandlers;

namespace DestilatechPlatform.API.AlertsNotifications.Application.Internal.EventHandlers;

public class AlertRaisedEventHandler(ILogger<AlertRaisedEventHandler> logger) : IEventHandler<AlertRaisedEvent>
{
    public Task Handle(AlertRaisedEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Alert raised for account {AccountId}: {Type} on source {SourceId} with severity {Severity}",
            domainEvent.AccountId.Value, domainEvent.Type, domainEvent.SourceId.Value, domainEvent.Severity);
        return Task.CompletedTask;
    }
}