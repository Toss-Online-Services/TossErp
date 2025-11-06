using Microsoft.Extensions.Logging;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Events;

namespace Toss.Application.Sales.EventHandlers;

/// <summary>
/// Handles customer purchase history updates when a sale is completed.
/// </summary>
public class SaleCompletedCustomerUpdateEventHandler : INotificationHandler<SaleCompletedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<SaleCompletedCustomerUpdateEventHandler> _logger;

    public SaleCompletedCustomerUpdateEventHandler(
        IApplicationDbContext context,
        ILogger<SaleCompletedCustomerUpdateEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(SaleCompletedEvent notification, CancellationToken cancellationToken)
    {
        // Reload sale to ensure proper tracking
        var sale = await _context.Sales
            .FirstOrDefaultAsync(s => s.Id == notification.Sale.Id, cancellationToken);

        if (sale == null)
        {
            _logger.LogWarning(
                "Sale {SaleId} not found in database",
                notification.Sale.Id);
            return;
        }

        // Update customer purchase history if customer is linked
        if (!sale.CustomerId.HasValue)
        {
            _logger.LogDebug(
                "Sale {SaleNumber} has no customer - skipping customer update",
                sale.SaleNumber);
            return;
        }

        _logger.LogInformation(
            "Updating customer purchase history for sale {SaleNumber}, customer {CustomerId}",
            sale.SaleNumber,
            sale.CustomerId.Value);

        var customer = await _context.Customers.FindAsync(new object[] { sale.CustomerId.Value }, cancellationToken);
        if (customer != null)
        {
            customer.TotalPurchaseAmount += sale.Total;
            customer.TotalPurchaseCount += 1;
            customer.LastPurchaseDate = sale.SaleDate;

            if (!customer.FirstPurchaseDate.HasValue)
                customer.FirstPurchaseDate = sale.SaleDate;

            _logger.LogInformation(
                "Successfully updated customer {CustomerId} purchase history. Total purchases: {Count}, Total amount: {Amount}",
                customer.Id,
                customer.TotalPurchaseCount,
                customer.TotalPurchaseAmount);

            try
            {
                // Save all changes to the database
                var savedCount = await _context.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation(
                    "Saved customer update for sale {SaleNumber}. Saved {Count} changes",
                    sale.SaleNumber,
                    savedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to save customer update for sale {SaleNumber}",
                    sale.SaleNumber);
                throw; // Re-throw to ensure the error is not silently swallowed
            }
        }
        else
        {
            _logger.LogWarning(
                "Customer {CustomerId} not found for sale {SaleNumber}",
                sale.CustomerId.Value,
                sale.SaleNumber);
        }
    }
}

