using eShop.POS.Domain.AggregatesModel.SaleAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.POS.API.EventHandlers;

public class SaleCompletedEventHandler : INotificationHandler<SaleCompletedDomainEvent>
{
    private readonly ILogger<SaleCompletedEventHandler> _logger;
    private readonly IInventoryService _inventoryService;
    private readonly IAnalyticsService _analyticsService;
    private readonly INotificationService _notificationService;

    public SaleCompletedEventHandler(
        ILogger<SaleCompletedEventHandler> logger,
        IInventoryService inventoryService,
        IAnalyticsService analyticsService,
        INotificationService notificationService)
    {
        _logger = logger;
        _inventoryService = inventoryService;
        _analyticsService = analyticsService;
        _notificationService = notificationService;
    }

    public async Task Handle(SaleCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling SaleCompletedDomainEvent for Sale {SaleId}", notification.SaleId);

        try
        {
            // Update inventory
            await _inventoryService.UpdateStockForSale(notification.SaleId, cancellationToken);

            // Update analytics
            await _analyticsService.TrackSaleCompleted(
                notification.StoreId,
                notification.StaffId,
                notification.Total,
                notification.IsOffline,
                cancellationToken);

            // Send notifications if needed
            if (notification.Total > 1000) // Example threshold for high-value sales
            {
                await _notificationService.NotifyHighValueSale(
                    notification.StoreId,
                    notification.SaleId,
                    notification.Total,
                    cancellationToken);
            }

            _logger.LogInformation("Successfully handled SaleCompletedDomainEvent for Sale {SaleId}", notification.SaleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling SaleCompletedDomainEvent for Sale {SaleId}", notification.SaleId);
            throw;
        }
    }
} 
