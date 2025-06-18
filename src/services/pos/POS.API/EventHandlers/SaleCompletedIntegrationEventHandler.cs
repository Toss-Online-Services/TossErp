using eShop.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using POS.API.Application.IntegrationEvents.Events;

namespace POS.API.EventHandlers;

public class SaleCompletedIntegrationEventHandler : IIntegrationEventHandler<SaleCompletedIntegrationEvent>
{
    private readonly ILogger<SaleCompletedIntegrationEventHandler> _logger;

    public SaleCompletedIntegrationEventHandler(ILogger<SaleCompletedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleCompletedIntegrationEvent @event)
    {
        _logger.LogInformation("Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", 
            @event.Id, "POS.API", @event);

        // Handle the sale completed event
        // This could trigger inventory updates, financial reporting, customer loyalty updates, etc.
        
        return Task.CompletedTask;
    }
} 
