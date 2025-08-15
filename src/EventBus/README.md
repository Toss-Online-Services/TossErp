# EventBus Implementation

This project provides a MassTransit-based event bus implementation for the TossErp microservices architecture.

## Features

- **MassTransit Integration**: Uses MassTransit for reliable message processing
- **RabbitMQ Transport**: RabbitMQ as the message broker
- **Integration Events**: Support for cross-service communication
- **Domain Event Dispatching**: Automatic conversion of domain events to integration events
- **Health Checks**: Built-in health monitoring for the event bus
- **Retry Policies**: Configurable retry mechanisms for failed messages
- **Error Handling**: Comprehensive error handling and logging

## Architecture

### Components

1. **EventBus**: Main implementation using MassTransit's `IPublishEndpoint`
2. **IntegrationEventConsumer**: Generic consumer for integration events
3. **DomainEventDispatcher**: Converts domain events to integration events
4. **EventBusHealthCheck**: Health monitoring for the event bus
5. **Configuration**: EventBus configuration settings

### Event Flow

```
Domain Event → DomainEventDispatcher → Integration Event → EventBus → RabbitMQ → Consumer → Handler
```

## Configuration

### appsettings.json

```json
{
  "EventBus": {
    "EventBusConnection": "amqp://guest:guest@localhost:5672",
    "EventBusUserName": "guest",
    "EventBusPassword": "guest",
    "EventBusRetryCount": "5",
    "SubscriptionClientName": "YourService.API"
  }
}
```

### Service Registration

```csharp
// In your service's DependencyInjection.cs
services.AddMassTransitEventBus(configuration);

// Register domain event dispatcher
services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

// Register health checks
services.AddHealthChecks()
    .AddCheck<EventBusHealthCheck>("eventbus", tags: new[] { "eventbus", "messaging" });
```

## Usage

### Publishing Integration Events

```csharp
public class YourService
{
    private readonly IEventBus _eventBus;

    public YourService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task PublishEventAsync()
    {
        var integrationEvent = new YourIntegrationEvent(
            // event properties
        );

        await _eventBus.PublishAsync(integrationEvent);
    }
}
```

### Creating Integration Events

```csharp
public record YourIntegrationEvent(
    Guid Id,
    string Name,
    DateTime CreatedAt
) : IntegrationEvent;
```

### Creating Integration Event Handlers

```csharp
public class YourIntegrationEventHandler : IIntegrationEventHandler<YourIntegrationEvent>
{
    public async Task Handle(YourIntegrationEvent @event)
    {
        // Handle the integration event
        // This could involve updating local state, calling external APIs, etc.
    }
}
```

## Domain Event to Integration Event Mapping

The `DomainEventDispatcher` automatically converts domain events to integration events. To add new mappings:

1. Create your domain event
2. Create corresponding integration event
3. Add mapping in `DomainEventDispatcher.ConvertToIntegrationEvent()`

```csharp
private IntegrationEvent? ConvertToIntegrationEvent(IDomainEvent domainEvent)
{
    return domainEvent switch
    {
        ItemCreatedEvent itemCreated => new ItemCreatedIntegrationEvent(
            itemCreated.ItemId,
            itemCreated.ItemName,
            // ... other properties
        ),
        _ => null
    };
}
```

## Health Monitoring

The EventBus includes health checks that can be accessed via:

- `/health` endpoint
- Health check tags: `eventbus`, `messaging`

## Local Development

### Running RabbitMQ

```bash
# Using Docker Compose
docker-compose -f docker-compose.eventbus.yml up -d

# Or using Docker directly
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

### RabbitMQ Management UI

Access the RabbitMQ management interface at: http://localhost:15672
- Username: guest
- Password: guest

## Best Practices

1. **Event Design**: Keep integration events focused and include only necessary data
2. **Error Handling**: Always handle exceptions in event handlers
3. **Idempotency**: Design handlers to be idempotent
4. **Monitoring**: Use health checks and logging for monitoring
5. **Testing**: Test event publishing and consumption in isolation

## Troubleshooting

### Common Issues

1. **Connection Failed**: Check RabbitMQ is running and connection string is correct
2. **Events Not Published**: Verify EventBus is registered in DI container
3. **Handlers Not Called**: Ensure handlers are registered and consumer is configured

### Logging

Enable detailed logging for debugging:

```json
{
  "Logging": {
    "LogLevel": {
      "eShop.EventBus": "Debug",
      "MassTransit": "Debug"
    }
  }
}
```
