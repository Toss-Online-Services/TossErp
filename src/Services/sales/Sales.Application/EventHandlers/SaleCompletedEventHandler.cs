using MediatR;
using TossErp.Sales.Application.Common.Interfaces;
using TossErp.Sales.Domain.Events;

namespace TossErp.Sales.Application.EventHandlers;

/// <summary>
/// Handler for SaleCompleted domain events
/// </summary>
public class SaleCompletedEventHandler : INotificationHandler<SaleCompletedEvent>
{
    private readonly IInventoryService _inventoryService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<SaleCompletedEventHandler> _logger;

    public SaleCompletedEventHandler(
        IInventoryService inventoryService,
        INotificationService notificationService,
        ILogger<SaleCompletedEventHandler> logger)
    {
        _inventoryService = inventoryService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(SaleCompletedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling SaleCompleted event for sale {SaleId}", notification.SaleId);

            // Update inventory stock levels
            await UpdateInventoryStock(notification, cancellationToken);

            // Send notifications
            await SendNotifications(notification, cancellationToken);

            // Log the completion
            _logger.LogInformation("Sale {SaleId} completed successfully. Total: {Total}, Paid: {PaidAmount}, Change: {ChangeAmount}",
                notification.SaleId, notification.Total.Amount, notification.PaidAmount.Amount, notification.ChangeAmount.Amount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling SaleCompleted event for sale {SaleId}", notification.SaleId);
            throw;
        }
    }

    private async Task UpdateInventoryStock(SaleCompletedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            // For MVP, we'll simulate inventory updates
            // In a real implementation, this would call the inventory service
            _logger.LogInformation("Updating inventory stock for sale {SaleId}", notification.SaleId);
            
            // TODO: Get sale items from repository and update inventory
            // var sale = await _saleRepository.GetByIdAsync(notification.SaleId, cancellationToken);
            // if (sale != null)
            // {
            //     await _inventoryService.UpdateStockLevelsAsync(notification.SaleId, sale.Items, cancellationToken);
            // }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating inventory stock for sale {SaleId}", notification.SaleId);
            // Don't rethrow - inventory update failure shouldn't fail the sale completion
        }
    }

    private async Task SendNotifications(SaleCompletedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            // Send notification to store manager
            var subject = $"Sale Completed - Receipt #{notification.SaleId}";
            var message = $"A sale has been completed with total amount: {notification.Total.Amount:C}. " +
                         $"Paid amount: {notification.PaidAmount.Amount:C}. " +
                         $"Change amount: {notification.ChangeAmount.Amount:C}.";

            await _notificationService.SendNotificationAsync(
                "store-manager", // This would be the actual manager's ID
                subject,
                message,
                cancellationToken);

            _logger.LogInformation("Sent completion notification for sale {SaleId}", notification.SaleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notifications for sale {SaleId}", notification.SaleId);
            // Don't rethrow - notification failure shouldn't fail the sale completion
        }
    }
}
