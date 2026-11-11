using DestilatechPlatform.API.Shared.Domain.Model.Events;

namespace DestilatechPlatform.API.Shared.Domain.Model.Aggregates.Internal;

public abstract class AggregateRoot : IAggregateRoot
{
    private readonly List<IEvent> _domainEvents = [];

    public IReadOnlyCollection<IEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}