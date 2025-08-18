using TossErp.Procurement.Domain.Events;

namespace TossErp.Procurement.Application.Common.Interfaces;

/// <summary>
/// Service for publishing domain events
/// </summary>
public interface IDomainEventService
{
    /// <summary>
    /// Publish domain events
    /// </summary>
    /// <param name="domainEvents">Domain events to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task PublishAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publish a single domain event
    /// </summary>
    /// <param name="domainEvent">Domain event to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
