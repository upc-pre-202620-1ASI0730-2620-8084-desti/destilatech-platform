using DestilatechPlatform.API.Shared.Domain.Model.Events;

namespace DestilatechPlatform.API.Shared.Domain.Model.Aggregates;

public interface IAggregateRoot
{
    IReadOnlyCollection<IEvent> DomainEvents { get; }
    void ClearDomainEvents();
}