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
            // Map TossErp domain events to integration events
            TossErp.Stock.Domain.Events.ItemCreatedEvent itemCreated => new eShop.EventBus.Events.Stock.ItemCreatedIntegrationEvent(
                itemCreated.ItemId,
                itemCreated.SKU, // ItemCode
                itemCreated.Name, // ItemName
                null, // Description - not available in domain event
                itemCreated.Category,
                string.Empty, // Unit - not available in domain event
                0, // StandardRate - not available in domain event
                0, // MinimumPrice - not available in domain event
                null, // WeightPerUnit - not available in domain event
                null, // Length - not available in domain event
                null, // Width - not available in domain event
                null, // Height - not available in domain event
                true, // IsActive - assume true for new items
                DateTime.UtcNow, // CreatedAt
                null), // CreatedBy - not available in domain event
            
            TossErp.Stock.Domain.Events.StockReceivedEvent stockReceived => new eShop.EventBus.Events.Stock.StockReceivedIntegrationEvent(
                Guid.NewGuid(), // StockMovementId - generate new ID
                stockReceived.ItemId,
                string.Empty, // ItemCode - would need to be retrieved from repository
                string.Empty, // ItemName - would need to be retrieved from repository
                stockReceived.WarehouseId,
                string.Empty, // WarehouseCode - would need to be retrieved from repository
                string.Empty, // WarehouseName - would need to be retrieved from repository
                null, // BinId - not available in domain event
                null, // BinCode - not available in domain event
                stockReceived.Qty.Value, // Quantity
                0, // UnitCost - not available in domain event
                null, // BatchNo - not available in domain event
                null, // SerialNo - not available in domain event
                null, // ExpiryDate - not available in domain event
                "Receipt", // MovementType
                stockReceived.RefId ?? string.Empty, // VoucherNo
                stockReceived.RefType, // ReferenceDocumentType
                stockReceived.RefId, // ReferenceDocumentNo
                DateTime.UtcNow, // ReceivedAt
                null), // ReceivedBy - not available in domain event
            
            TossErp.Stock.Domain.Events.StockLevelLowEvent stockLevelLow => new eShop.EventBus.Events.Stock.StockLevelLowIntegrationEvent(
                stockLevelLow.ItemId,
                string.Empty, // ItemCode - would need to be retrieved from repository
                string.Empty, // ItemName - would need to be retrieved from repository
                string.Empty, // Category - would need to be retrieved from repository
                stockLevelLow.WarehouseId,
                string.Empty, // WarehouseCode - would need to be retrieved from repository
                string.Empty, // WarehouseName - would need to be retrieved from repository
                null, // BinId - not available in domain event
                null, // BinCode - not available in domain event
                stockLevelLow.CurrentQty.Value, // CurrentQuantity
                10, // Threshold - default value
                stockLevelLow.ReorderLevel.Value, // ReorderLevel
                0, // UnitCost - not available in domain event
                0, // TotalValue - not available in domain event
                DateTime.UtcNow, // DetectedAt
                null), // DetectedBy - not available in domain event
            
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
