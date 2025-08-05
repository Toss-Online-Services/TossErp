using Microsoft.Extensions.Logging;
using eShop.EventBus.Abstractions;
using eShop.EventBus.Events;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate.Entities;
using TossErp.Stock.Domain.ValueObjects;
using TossErp.Stock.Domain.Enums;

namespace TossErp.Stock.Application.IntegrationEvents;

/// <summary>
/// Integration event handler for PurchaseOrderCompletedIntegrationEvent
/// Creates stock entries when purchase orders are completed
/// </summary>
public class PurchaseOrderCompletedIntegrationEventHandler : IIntegrationEventHandler<PurchaseOrderCompletedIntegrationEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<PurchaseOrderCompletedIntegrationEventHandler> _logger;

    public PurchaseOrderCompletedIntegrationEventHandler(
        IApplicationDbContext context,
        ILogger<PurchaseOrderCompletedIntegrationEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(PurchaseOrderCompletedIntegrationEvent @event)
    {
        _logger.LogInformation("Processing PurchaseOrderCompletedIntegrationEvent for PO {PurchaseOrderNumber}", 
            @event.PurchaseOrderNumber);

        try
        {
            // Create stock entry for the purchase receipt
            var stockEntry = new StockEntryAggregate(
                new StockEntryNo($"PR-{@event.PurchaseOrderNumber}"),
                @event.CompletionDate,
                "Default", // Company
                $"Purchase Receipt - {@event.PurchaseOrderNumber}",
                $"Purchase receipt for PO {@event.PurchaseOrderNumber}");

            // Add stock entry details for each item
            foreach (var item in @event.Items)
            {
                var stockEntryDetail = new StockEntryDetail(
                    stockEntry.Id,
                    Guid.Parse(item.ItemCode), // Assuming ItemCode is a GUID
                    Guid.Parse(item.WarehouseCode), // Assuming WarehouseCode is a GUID
                    new Quantity(item.QuantityReceived),
                    new Rate(item.UnitPrice, @event.Currency),
                    batchNo: item.BatchNumber,
                    expiryDate: item.ExpiryDate);

                // Set serial number if available
                if (item.SerialNumbers.Any())
                {
                    stockEntryDetail.UpdateSerialNo(item.SerialNumbers.First());
                }

                stockEntry.AddDetail(stockEntryDetail);
            }

            // Add to context
            _context.StockEntries.Add(stockEntry);

            // Save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully processed purchase order completion for PO {PurchaseOrderNumber} with {ItemCount} items", 
                @event.PurchaseOrderNumber, @event.Items.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing PurchaseOrderCompletedIntegrationEvent for PO {PurchaseOrderNumber}", 
                @event.PurchaseOrderNumber);
            throw;
        }
    }
} 
