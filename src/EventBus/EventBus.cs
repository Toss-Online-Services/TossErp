using System.Text.Json;
using eShop.EventBus.Abstractions;
using eShop.EventBus.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace eShop.EventBus;

public class EventBus : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<EventBus> _logger;

    public EventBus(IPublishEndpoint publishEndpoint, ILogger<EventBus> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAsync(IntegrationEvent @event)
    {
        try
        {
            _logger.LogInformation("Publishing integration event: {EventType} with ID {EventId}", 
                @event.GetType().Name, @event.Id);

            await _publishEndpoint.Publish(@event);

            _logger.LogInformation("Successfully published integration event: {EventType} with ID {EventId}", 
                @event.GetType().Name, @event.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing integration event: {EventType} with ID {EventId}", 
                @event.GetType().Name, @event.Id);
            throw;
        }
    }
}
