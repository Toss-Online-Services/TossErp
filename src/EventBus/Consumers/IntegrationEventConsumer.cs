using eShop.EventBus.Abstractions;
using eShop.EventBus.Events;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eShop.EventBus.Consumers;

public class IntegrationEventConsumer<T> : IConsumer<T> where T : IntegrationEvent
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IntegrationEventConsumer<T>> _logger;

    public IntegrationEventConsumer(
        IServiceProvider serviceProvider,
        ILogger<IntegrationEventConsumer<T>> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<T> context)
    {
        var @event = context.Message;
        
        _logger.LogInformation("Consuming integration event: {EventType} with ID {EventId}", 
            @event.GetType().Name, @event.Id);

        try
        {
            // Get the handler for this event type
            var handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(typeof(T));
            var handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
            {
                _logger.LogWarning("No handler found for integration event: {EventType}", @event.GetType().Name);
                return;
            }

            // Invoke the handler
            var handleMethod = handlerType.GetMethod("Handle");
            if (handleMethod != null)
            {
                await (Task)handleMethod.Invoke(handler, new object[] { @event })!;
            }

            _logger.LogInformation("Successfully consumed integration event: {EventType} with ID {EventId}", 
                @event.GetType().Name, @event.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming integration event: {EventType} with ID {EventId}", 
                @event.GetType().Name, @event.Id);
            throw;
        }
    }
}
