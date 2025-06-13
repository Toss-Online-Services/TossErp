using eShop.POS.Domain.AggregatesModel.SaleAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.POS.API.EventHandlers;

public class SaleSyncedEventHandler : INotificationHandler<SaleSyncedDomainEvent>
{
    private readonly ILogger<SaleSyncedEventHandler> _logger;
    private readonly IAnalyticsService _analyticsService;
    private readonly ISyncService _syncService;

    public SaleSyncedEventHandler(
        ILogger<SaleSyncedEventHandler> logger,
        IAnalyticsService analyticsService,
        ISyncService syncService)
    {
        _logger = logger;
        _analyticsService = analyticsService;
        _syncService = syncService;
    }

    public async Task Handle(SaleSyncedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling SaleSyncedDomainEvent for Sale {SaleId}", notification.SaleId);

        try
        {
            // Update sync status
            await _syncService.MarkSaleAsSynced(
                notification.SaleId,
                notification.SyncedAt,
                cancellationToken);

            // Update analytics
            await _analyticsService.TrackSaleSynced(
                notification.StoreId,
                notification.StaffId,
                notification.SyncedAt,
                cancellationToken);

            _logger.LogInformation("Successfully handled SaleSyncedDomainEvent for Sale {SaleId}", notification.SaleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling SaleSyncedDomainEvent for Sale {SaleId}", notification.SaleId);
            throw;
        }
    }
} 
