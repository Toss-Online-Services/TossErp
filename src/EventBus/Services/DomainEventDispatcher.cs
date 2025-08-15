#nullable enable

using eShop.EventBus.Abstractions;
using eShop.EventBus.Events;
using Microsoft.Extensions.Logging;

namespace eShop.EventBus.Services;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(IEventBus eventBus, ILogger<DomainEventDispatcher> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task DispatchDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            try
            {
                _logger.LogInformation("Dispatching domain event: {EventType}", domainEvent.GetType().Name);

                // Convert domain event to integration event if possible
                var integrationEvent = ConvertToIntegrationEvent(domainEvent);
                if (integrationEvent != null)
                {
                    await _eventBus.PublishAsync(integrationEvent);
                    _logger.LogInformation("Successfully published integration event for domain event: {EventType}", 
                        domainEvent.GetType().Name);
                }
                else
                {
                    _logger.LogDebug("No integration event mapping found for domain event: {EventType}", 
                        domainEvent.GetType().Name);
                }
            }
            catch (NotSupportedException)
            {
                _logger.LogDebug("No integration event mapping found for domain event: {EventType}", 
                    domainEvent.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error dispatching domain event: {EventType}", domainEvent.GetType().Name);
                throw;
            }
        }
    }

    private IntegrationEvent? ConvertToIntegrationEvent(IDomainEvent domainEvent)
    {
        // This is a simple mapping approach. In a real application, you might want to:
        // 1. Use AutoMapper for complex mappings
        // 2. Have explicit mapping classes
        // 3. Use a factory pattern

        return domainEvent switch
        {
            // Add mappings for specific domain events to integration events
            // Note: These are placeholder mappings - you'll need to implement the actual domain events
            // and their properties to make these work correctly
            
            // Example mappings (uncomment and implement when domain events are available):
            // ItemCreatedEvent itemCreated => new ItemCreatedIntegrationEvent(
            //     itemCreated.ItemId,
            //     itemCreated.ItemCode,
            //     itemCreated.ItemName,
            //     itemCreated.Description,
            //     itemCreated.Category,
            //     itemCreated.Unit,
            //     itemCreated.StandardRate,
            //     itemCreated.MinimumPrice,
            //     itemCreated.WeightPerUnit,
            //     itemCreated.Length,
            //     itemCreated.Width,
            //     itemCreated.Height,
            //     itemCreated.IsActive,
            //     itemCreated.CreatedAt,
            //     itemCreated.CreatedBy),
            
            // StockLevelUpdatedEvent stockUpdated => new StockLevelUpdatedIntegrationEvent(
            //     stockUpdated.ItemId,
            //     stockUpdated.ItemCode,
            //     stockUpdated.ItemName,
            //     stockUpdated.WarehouseId,
            //     stockUpdated.WarehouseCode,
            //     stockUpdated.WarehouseName,
            //     stockUpdated.BinId,
            //     stockUpdated.BinCode,
            //     stockUpdated.PreviousQuantity,
            //     stockUpdated.NewQuantity,
            //     stockUpdated.ReservedQuantity,
            //     stockUpdated.AvailableQuantity,
            //     stockUpdated.UnitCost,
            //     stockUpdated.UpdatedAt,
            //     stockUpdated.UpdatedBy),
            
            _ => null
        };
    }
}

public interface IDomainEventDispatcher
{
    Task DispatchDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents);
}

// Generic domain event interface that can be implemented by any domain
public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}
