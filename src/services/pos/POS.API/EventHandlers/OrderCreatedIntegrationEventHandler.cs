using eShop.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using POS.API.Application.IntegrationEvents.Events;

namespace POS.API.EventHandlers;

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    private readonly ILogger<OrderCreatedIntegrationEventHandler> _logger;

    public OrderCreatedIntegrationEventHandler(ILogger<OrderCreatedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderCreatedIntegrationEvent @event)
    {
        _logger.LogInformation("Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", 
            @event.Id, "POS.API", @event);

        // Handle the order created event
        // This could trigger inventory updates, customer notifications, etc.
        
        return Task.CompletedTask;
    }
} 
