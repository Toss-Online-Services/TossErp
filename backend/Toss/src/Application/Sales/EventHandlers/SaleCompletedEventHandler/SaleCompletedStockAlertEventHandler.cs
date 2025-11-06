using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Events;

namespace Toss.Application.Sales.EventHandlers;

/// <summary>
/// Handles stock alert creation when stock levels fall below minimum threshold after a sale.
/// </summary>
public class SaleCompletedStockAlertEventHandler : INotificationHandler<SaleCompletedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<SaleCompletedStockAlertEventHandler> _logger;

    public SaleCompletedStockAlertEventHandler(
        IApplicationDbContext context,
        ILogger<SaleCompletedStockAlertEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(SaleCompletedEvent notification, CancellationToken cancellationToken)
    {
        // Reload sale with items to ensure proper tracking and data
        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == notification.Sale.Id, cancellationToken);

        if (sale == null)
        {
            _logger.LogWarning(
                "Sale {SaleId} not found in database",
                notification.Sale.Id);
            return;
        }

        _logger.LogInformation(
            "Checking stock alerts for sale {SaleNumber}",
            sale.SaleNumber);

        // Check if stock is low and create alert if needed
        var alertsAdded = false;
        foreach (var item in sale.Items)
        {
            var stockLevel = await _context.StockLevels
                .FirstOrDefaultAsync(sl => sl.ShopId == sale.ShopId && sl.ProductId == item.ProductId, cancellationToken);

            if (stockLevel == null)
            {
                continue;
            }

            var product = await _context.Products.FindAsync(new object[] { item.ProductId }, cancellationToken);
            if (product != null && stockLevel.CurrentStock <= product.MinimumStockLevel)
            {
                var existingAlert = await _context.StockAlerts
                    .FirstOrDefaultAsync(a => a.ShopId == sale.ShopId
                        && a.ProductId == item.ProductId
                        && !a.IsAcknowledged, cancellationToken);

                if (existingAlert == null)
                {
                    var alert = new StockAlert
                    {
                        ShopId = sale.ShopId,
                        ProductId = item.ProductId,
                        CurrentStock = stockLevel.CurrentStock,
                        MinimumStock = product.MinimumStockLevel,
                        IsAcknowledged = false
                    };

                    _context.StockAlerts.Add(alert);
                    alertsAdded = true;

                    _logger.LogWarning(
                        "Created stock alert for product {ProductId} in shop {ShopId}. Current stock: {CurrentStock}, Minimum: {MinimumStock}",
                        item.ProductId,
                        sale.ShopId,
                        stockLevel.CurrentStock,
                        product.MinimumStockLevel);
                }
            }
        }

        // Only save if we actually added any alerts
        if (alertsAdded)
        {
            try
            {
                var savedCount = await _context.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation(
                    "Completed stock alert check for sale {SaleNumber}. Saved {Count} changes",
                    sale.SaleNumber,
                    savedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to save stock alerts for sale {SaleNumber}",
                    sale.SaleNumber);
                throw; // Re-throw to ensure the error is not silently swallowed
            }
        }
        else
        {
            _logger.LogInformation(
                "No stock alerts to save for sale {SaleNumber}",
                sale.SaleNumber);
        }
    }
}

