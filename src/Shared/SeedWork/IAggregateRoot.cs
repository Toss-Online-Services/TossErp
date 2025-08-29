namespace TossErp.Shared.SeedWork;

/// <summary>
/// Base interface for aggregate root entities
/// </summary>
public interface IAggregateRoot : IEntity<Guid>
{
    /// <summary>
    /// Gets all domain events that have been raised by this aggregate
    /// </summary>
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Adds a domain event to be raised
    /// </summary>
    void AddDomainEvent(IDomainEvent domainEvent);

    /// <summary>
    /// Removes a specific domain event
    /// </summary>
    void RemoveDomainEvent(IDomainEvent domainEvent);

    /// <summary>
    /// Clears all domain events
    /// </summary>
    void ClearDomainEvents();
}

/// <summary>
/// Base abstract class for aggregate root entities
/// </summary>
public abstract class AggregateRoot : Entity<Guid>, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
