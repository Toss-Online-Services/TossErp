using MediatR;
using TossErp.Procurement.Application.Common.Interfaces;
using TossErp.Procurement.Domain.Events;

namespace TossErp.Procurement.API.Services;

/// <summary>
/// Implementation of IDomainEventService using MediatR
/// </summary>
public class DomainEventService : IDomainEventService
{
    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventService> _logger;

    public DomainEventService(IMediator mediator, ILogger<DomainEventService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task PublishAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            await PublishAsync(domainEvent, cancellationToken);
        }
    }

    public async Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Publishing domain event: {EventType}", domainEvent.GetType().Name);
        
        // Publish via MediatR
        await _mediator.Publish(domainEvent, cancellationToken);
    }
}
