using POS.Domain.Common.Events;

namespace POS.Domain.SeedWork;

public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public new IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public new void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
} 
