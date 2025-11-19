using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Enums;
using Toss.Domain.Events;

namespace Toss.Application.Sales.EventHandlers;

/// <summary>
/// Handles stock level updates and stock movement creation when a sale is completed.
/// </summary>
public class SaleCompletedStockUpdateEventHandler : INotificationHandler<SaleCompletedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<SaleCompletedStockUpdateEventHandler> _logger;

    public SaleCompletedStockUpdateEventHandler(
        IApplicationDbContext context,
        ILogger<SaleCompletedStockUpdateEventHandler> logger)
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
            "Updating stock levels for sale {SaleNumber}",
            sale.SaleNumber);

        // Update stock levels and create stock movements
        foreach (var item in sale.Items)
        {
            // Get stock level
            var stockLevel = await _context.StockLevels
                .FirstOrDefaultAsync(sl => sl.ShopId == sale.ShopId && sl.ProductId == item.ProductId, cancellationToken);

            if (stockLevel == null)
            {
                _logger.LogWarning(
                    "Stock level not found for product {ProductId} in shop {ShopId}",
                    item.ProductId,
                    sale.ShopId);
                continue;
            }

            var previousStock = stockLevel.CurrentStock;
            stockLevel.CurrentStock -= item.Quantity;
            stockLevel.LastStockDate = DateTimeOffset.UtcNow;

            _logger.LogDebug(
                "Updating stock for product {ProductId}: {PreviousStock} -> {NewStock} (quantity: {Quantity})",
                item.ProductId,
                previousStock,
                stockLevel.CurrentStock,
                item.Quantity);

            // Create stock movement record
            var stockMovement = new StockMovement
            {
                ShopId = sale.ShopId,
                ProductId = item.ProductId,
                MovementType = StockMovementType.Sale,
                QuantityBefore = previousStock,
                QuantityChange = -item.Quantity,
                QuantityAfter = stockLevel.CurrentStock,
                ReferenceType = "Sale",
                ReferenceId = sale.Id,
                Notes = $"Sale {sale.SaleNumber}",
                MovementDate = sale.SaleDate
            };

            _context.StockMovements.Add(stockMovement);
        }

        // We always have changes (stock level updates and movements), so save them
        try
        {
            // Log what we're about to save
            var modifiedStockLevels = sale.Items.Count;
            var newMovements = sale.Items.Count;
            _logger.LogDebug(
                "About to save: {StockLevels} stock level updates and {Movements} stock movements for sale {SaleNumber}",
                modifiedStockLevels,
                newMovements,
                sale.SaleNumber);

            // Save all changes to the database
            var savedCount = await _context.SaveChangesAsync(cancellationToken);
            
            if (savedCount == 0)
            {
                _logger.LogWarning(
                    "SaveChangesAsync returned 0 for sale {SaleNumber} - no changes were saved!",
                    sale.SaleNumber);
            }
            else
            {
                _logger.LogInformation(
                    "Successfully updated stock levels for sale {SaleNumber}. Saved {Count} changes",
                    sale.SaleNumber,
                    savedCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to save stock updates for sale {SaleNumber}. Exception: {ExceptionType} - {Message}",
                sale.SaleNumber,
                ex.GetType().Name,
                ex.Message);
            throw; // Re-throw to ensure the error is not silently swallowed
        }
    }
}

