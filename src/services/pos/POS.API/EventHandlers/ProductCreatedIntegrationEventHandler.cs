using eShop.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using POS.API.Application.IntegrationEvents.Events;

namespace POS.API.EventHandlers;

public class ProductCreatedIntegrationEventHandler : IIntegrationEventHandler<ProductCreatedIntegrationEvent>
{
    private readonly ILogger<ProductCreatedIntegrationEventHandler> _logger;

    public ProductCreatedIntegrationEventHandler(ILogger<ProductCreatedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ProductCreatedIntegrationEvent @event)
    {
        _logger.LogInformation("Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", 
            @event.Id, "POS.API", @event);

        // Handle the product created event
        // This could trigger inventory updates, notifications, etc.
        
        return Task.CompletedTask;
    }
} 
