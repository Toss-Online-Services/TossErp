using eShop.POS.Domain.AggregatesModel.SaleAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.POS.API.EventHandlers;

public class SaleDiscountAddedEventHandler : INotificationHandler<SaleDiscountAddedDomainEvent>
{
    private readonly ILogger<SaleDiscountAddedEventHandler> _logger;
    private readonly IAnalyticsService _analyticsService;
    private readonly IStaffService _staffService;

    public SaleDiscountAddedEventHandler(
        ILogger<SaleDiscountAddedEventHandler> logger,
        IAnalyticsService analyticsService,
        IStaffService staffService)
    {
        _logger = logger;
        _analyticsService = analyticsService;
        _staffService = staffService;
    }

    public async Task Handle(SaleDiscountAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling SaleDiscountAddedDomainEvent for Sale {SaleId}", notification.SaleId);

        try
        {
            // Update analytics
            await _analyticsService.TrackDiscountAdded(
                notification.StoreId,
                notification.StaffId,
                notification.Type,
                notification.Amount,
                cancellationToken);

            // Handle tips for staff
            if (notification.Type == DiscountType.Tip && !string.IsNullOrEmpty(notification.DiscountStaffId))
            {
                await _staffService.RecordTip(
                    notification.DiscountStaffId,
                    notification.Amount,
                    notification.SaleId,
                    cancellationToken);
            }

            _logger.LogInformation("Successfully handled SaleDiscountAddedDomainEvent for Sale {SaleId}", notification.SaleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling SaleDiscountAddedDomainEvent for Sale {SaleId}", notification.SaleId);
            throw;
        }
    }
} 
