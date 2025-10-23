using Microsoft.Extensions.Logging;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Inventory;
using Toss.Domain.Enums;
using Toss.Domain.Events;

namespace Toss.Application.Sales.EventHandlers;

public class SaleCompletedEventHandler : INotificationHandler<SaleCompletedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<SaleCompletedEventHandler> _logger;

    public SaleCompletedEventHandler(
        IApplicationDbContext context,
        ILogger<SaleCompletedEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(SaleCompletedEvent notification, CancellationToken cancellationToken)
    {
        var sale = notification.Sale;

        _logger.LogInformation("Processing sale completed event for sale {SaleNumber}", sale.SaleNumber);

        // Update stock levels and create stock movements
        foreach (var item in sale.Items)
        {
            // Get or create stock level
            var stockLevel = await _context.StockLevels
                .FirstOrDefaultAsync(sl => sl.ShopId == sale.ShopId && sl.ProductId == item.ProductId, cancellationToken);

            if (stockLevel == null)
            {
                _logger.LogWarning("Stock level not found for product {ProductId} in shop {ShopId}", item.ProductId, sale.ShopId);
                continue;
            }

            var previousStock = stockLevel.CurrentStock;
            stockLevel.CurrentStock -= item.Quantity;
            stockLevel.LastStockDate = DateTimeOffset.UtcNow;

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

            // Check if stock is low and create alert if needed
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
                }
            }
        }

        // Update customer purchase history if customer is linked
        if (sale.CustomerId.HasValue)
        {
            var customer = await _context.Customers.FindAsync(new object[] { sale.CustomerId.Value }, cancellationToken);
            if (customer != null)
            {
                customer.TotalPurchaseAmount += sale.Total;
                customer.TotalPurchaseCount += 1;
                customer.LastPurchaseDate = sale.SaleDate;

                if (!customer.FirstPurchaseDate.HasValue)
                    customer.FirstPurchaseDate = sale.SaleDate;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully processed sale completed event for sale {SaleNumber}", sale.SaleNumber);
    }
}

