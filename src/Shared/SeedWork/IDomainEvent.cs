namespace TossErp.Shared.SeedWork;

/// <summary>
/// Base interface for domain events
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Unique identifier for the event
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// When the event occurred
    /// </summary>
    DateTime OccurredOn { get; }
}

/// <summary>
/// Base abstract class for domain events
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; private set; } = DateTime.UtcNow;
}
