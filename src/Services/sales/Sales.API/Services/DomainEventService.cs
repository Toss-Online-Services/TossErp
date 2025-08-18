using MediatR;
using TossErp.Sales.Application.Common.Interfaces;
using TossErp.Sales.Domain.Events;

namespace TossErp.Sales.API.Services;

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
        try
        {
            _logger.LogInformation("Publishing domain event: {EventType} for {EventId}", 
                domainEvent.GetType().Name, domainEvent.Id);

            // Publish the domain event using MediatR
            await _mediator.Publish(domainEvent, cancellationToken);

            _logger.LogInformation("Successfully published domain event: {EventType} for {EventId}", 
                domainEvent.GetType().Name, domainEvent.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing domain event: {EventType} for {EventId}", 
                domainEvent.GetType().Name, domainEvent.Id);
            throw;
        }
    }
}
