using eShop.EventBus.Abstractions;

namespace POS.API.Application.IntegrationEvents;

public interface IPOSIntegrationEventService
{
    Task PublishEventsThroughEventBusAsync(Guid transactionId);
    Task AddAndSaveEventAsync(IntegrationEvent evt);
} 
