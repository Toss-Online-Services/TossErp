using eShop.EventBus.Abstractions;
using eShop.IntegrationEventLogEF.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using POS.Infrastructure;
using POS.Infrastructure.Extensions;
using System.Data.Common;

namespace POS.API.Application.IntegrationEvents;

public class POSIntegrationEventService : IPOSIntegrationEventService
{
    private readonly IEventBus _eventBus;
    private readonly POSContext _posContext;
    private readonly IIntegrationEventLogService _eventLogService;
    private readonly ILogger<POSIntegrationEventService> _logger;
    private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;

    public POSIntegrationEventService(
        IEventBus eventBus,
        POSContext posContext,
        Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
        ILogger<POSIntegrationEventService> logger)
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _posContext = posContext ?? throw new ArgumentNullException(nameof(posContext));
        _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _eventLogService = _integrationEventLogServiceFactory(_posContext.Database.GetDbConnection());
    }

    public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
    {
        var pendingLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

        foreach (var logEvt in pendingLogEvents)
        {
            _logger.LogInformation("Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})", logEvt.EventId, logEvt.IntegrationEvent);

            try
            {
                await _eventLogService.MarkEventAsInProgressAsync(logEvt.EventId);
                await _eventBus.PublishAsync(logEvt.IntegrationEvent);
                await _eventLogService.MarkEventAsPublishedAsync(logEvt.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing integration event: {IntegrationEventId}", logEvt.EventId);
                await _eventLogService.MarkEventAsFailedAsync(logEvt.EventId);
            }
        }
    }

    public async Task AddAndSaveEventAsync(IntegrationEvent evt)
    {
        _logger.LogInformation("Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", evt.Id, evt);

        await _eventLogService.SaveEventAsync(evt, _posContext.GetCurrentTransaction());
    }
} 
