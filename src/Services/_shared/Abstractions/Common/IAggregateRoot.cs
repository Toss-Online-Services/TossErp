namespace TossErp.Abstractions.Common;

/// <summary>
/// Marker interface for aggregate roots in Domain-Driven Design
/// </summary>
public interface IAggregateRoot : IEntity
{
    /// <summary>
    /// Domain events raised by this aggregate
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Clear all domain events
    /// </summary>
    void ClearDomainEvents();
}

/// <summary>
/// Base interface for domain events
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// When the event occurred
    /// </summary>
    DateTimeOffset OccurredOn { get; }
}
