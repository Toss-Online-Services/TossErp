# MassTransit + RabbitMQ Event Bus Setup

## Overview

Successfully configured MassTransit with RabbitMQ for the Stock module's event bus infrastructure. This enables cross-service communication through integration events and bridges domain events to integration events.

## Components Implemented

### 1. EventBus Project (`src/EventBus/`)

#### Core Components:
- **EventBus.cs**: Main implementation using MassTransit's `IPublishEndpoint`
- **IntegrationEventConsumer.cs**: Generic consumer for integration events
- **DomainEventDispatcher.cs**: Converts domain events to integration events
- **EventBusHealthCheck.cs**: Health monitoring for the event bus
- **EventBusConfiguration.cs**: Configuration settings

#### Extensions:
- **MassTransitEventBusBuilderExtensions.cs**: DI configuration for MassTransit
- **README.md**: Comprehensive documentation

### 2. Integration Events

#### Created Events:
- **ItemCreatedIntegrationEvent**: Published when items are created
- **StockLevelUpdatedIntegrationEvent**: Published when stock levels change

### 3. Configuration

#### AppSettings:
- **appsettings.json**: Production configuration
- **appsettings.Development.json**: Development configuration
- **docker-compose.eventbus.yml**: RabbitMQ and Redis containers

#### Package Management:
- Added MassTransit packages to central package management
- Configured versions in `src/Directory.Build.props`

### 4. Infrastructure Integration

#### Updated Components:
- **DependencyInjection.cs**: Registered MassTransit services
- **DispatchDomainEventsInterceptor.cs**: Enhanced to publish integration events
- **Repository implementations**: Added `GetQueryable()` method

## Architecture

### Event Flow:
```
Domain Event → DispatchDomainEventsInterceptor → DomainEventDispatcher → Integration Event → EventBus → RabbitMQ → Consumer → Handler
```

### Key Features:
- **Dual Event Publishing**: Domain events trigger both internal MediatR handlers and cross-service integration events
- **Generic EventBus**: Reusable across all microservices
- **Health Monitoring**: Built-in health checks for event bus connectivity
- **Retry Policies**: Configurable retry mechanisms for failed messages
- **Error Handling**: Comprehensive error handling and logging

## Configuration

### EventBus Settings:
```json
{
  "EventBus": {
    "EventBusConnection": "amqp://guest:guest@localhost:5672",
    "EventBusUserName": "guest",
    "EventBusPassword": "guest",
    "EventBusRetryCount": "5",
    "SubscriptionClientName": "Stock.API"
  }
}
```

### Local Development:
```bash
# Start RabbitMQ
docker-compose -f docker-compose.eventbus.yml up -d

# Access RabbitMQ Management UI
# http://localhost:15672 (guest/guest)
```

## Usage

### Publishing Integration Events:
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
        var integrationEvent = new YourIntegrationEvent(/* properties */);
        await _eventBus.PublishAsync(integrationEvent);
    }
}
```

### Creating Integration Event Handlers:
```csharp
public class YourIntegrationEventHandler : IIntegrationEventHandler<YourIntegrationEvent>
{
    public async Task Handle(YourIntegrationEvent @event)
    {
        // Handle the integration event
    }
}
```

## Domain Event to Integration Event Mapping

The `DomainEventDispatcher` automatically converts domain events to integration events. To add new mappings:

1. Create your domain event
2. Create corresponding integration event
3. Add mapping in `DomainEventDispatcher.ConvertToIntegrationEvent()`

Example:
```csharp
return domainEvent switch
{
    ItemCreatedEvent itemCreated => new ItemCreatedIntegrationEvent(
        itemCreated.ItemId,
        itemCreated.ItemCode,
        itemCreated.ItemName,
        // ... other properties
    ),
    _ => null
};
```

## Health Monitoring

- **Endpoint**: `/health`
- **Tags**: `eventbus`, `messaging`
- **Checks**: Configuration validation, connection status

## Next Steps

1. **Create Domain Events**: Implement actual domain events (e.g., `ItemCreatedEvent`, `StockLevelUpdatedEvent`)
2. **Add Event Mappings**: Update `DomainEventDispatcher` with real mappings
3. **Create Handlers**: Implement integration event handlers for cross-service communication
4. **Testing**: Add unit and integration tests for event publishing/consumption
5. **Monitoring**: Set up monitoring and alerting for event bus health

## Troubleshooting

### Common Issues:
1. **Connection Failed**: Check RabbitMQ is running and connection string is correct
2. **Events Not Published**: Verify EventBus is registered in DI container
3. **Handlers Not Called**: Ensure handlers are registered and consumer is configured

### Logging:
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

## Build Status

✅ **All projects compile successfully**
✅ **MassTransit packages properly configured**
✅ **EventBus infrastructure ready for use**
✅ **Health checks implemented**
✅ **Documentation complete**

The MassTransit + RabbitMQ Event Bus is now fully configured and ready for integration with the Stock module's domain events.
