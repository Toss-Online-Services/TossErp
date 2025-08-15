using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eShop.EventBus.Abstractions;
using eShop.EventBus.Events.Purchasing;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;

namespace TossErp.Stock.Application.EventHandlers;

/// <summary>
/// Handles PurchaseOrderReceived integration events by updating stock levels
/// </summary>
public class PurchaseOrderReceivedIntegrationEventHandler : IIntegrationEventHandler<PurchaseOrderReceivedIntegrationEvent>
{
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PurchaseOrderReceivedIntegrationEventHandler> _logger;

    public PurchaseOrderReceivedIntegrationEventHandler(
        IStockLevelRepository stockLevelRepository,
        IStockMovementRepository stockMovementRepository,
        IItemRepository itemRepository,
        IWarehouseRepository warehouseRepository,
        IUnitOfWork unitOfWork,
        ILogger<PurchaseOrderReceivedIntegrationEventHandler> logger)
    {
        _stockLevelRepository = stockLevelRepository;
        _stockMovementRepository = stockMovementRepository;
        _itemRepository = itemRepository;
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(PurchaseOrderReceivedIntegrationEvent @event)
    {
        try
        {
            _logger.LogInformation("Processing PurchaseOrderReceived event for PO {PurchaseOrderId}", @event.PurchaseOrderId);

            // Process each item in the purchase order
            foreach (var poItem in @event.Items)
            {
                await ProcessPurchaseOrderItem(@event, poItem);
            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Successfully processed PurchaseOrderReceived event for PO {PurchaseOrderId}", @event.PurchaseOrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing PurchaseOrderReceived event for PO {PurchaseOrderId}", @event.PurchaseOrderId);
            throw;
        }
    }

    private async Task ProcessPurchaseOrderItem(PurchaseOrderReceivedIntegrationEvent poEvent, PurchaseOrderItemDto poItem)
    {
        // Get the item to validate it exists
        var item = await _itemRepository.GetByIdAsync(poItem.ItemId);
        if (item == null)
        {
            _logger.LogWarning("Item {ItemId} not found for PO {PurchaseOrderId}", poItem.ItemId, poEvent.PurchaseOrderId);
            return;
        }

        // Determine warehouse to use (from PO item or default)
        var warehouseId = poItem.WarehouseId ?? await GetDefaultWarehouseId();
        if (warehouseId == null)
        {
            _logger.LogWarning("No warehouse specified for item {ItemId} in PO {PurchaseOrderId}", poItem.ItemId, poEvent.PurchaseOrderId);
            return;
        }

        // Get or create stock level
        var stockLevel = await _stockLevelRepository.GetByItemAndWarehouseAsync(poItem.ItemId, warehouseId.Value);
        if (stockLevel == null)
        {
            // Create new stock level if it doesn't exist
            stockLevel = StockLevel.Create(
                poItem.ItemId,
                warehouseId.Value,
                poItem.BinId,
                0, // Initial quantity
                poItem.UnitPrice // Unit cost
            );

            await _stockLevelRepository.AddAsync(stockLevel);
            _logger.LogInformation("Created new stock level for item {ItemId} in warehouse {WarehouseId}", poItem.ItemId, warehouseId);
        }

        // Create stock movement using the static factory method
        var stockMovement = StockMovement.CreateReceipt(
            Guid.NewGuid(), // TenantId - should come from context
            poItem.ItemId,
            warehouseId.Value,
            poItem.ReceivedQuantity,
            poEvent.ReceivedBy ?? "System", // CreatedBy
            poItem.BinId, // BinId
            poItem.UnitPrice, // UnitCost
            poEvent.PurchaseOrderId.ToString(), // ReferenceNumber
            "PurchaseOrder", // ReferenceType
            $"PO {poEvent.PurchaseOrderNumber}", // Reason
            null // BatchId - would need to be created if batch tracking is implemented
        );

        // Update stock level with received quantity
        stockLevel.ReceiveStock(poItem.ReceivedQuantity, poItem.UnitPrice);

        // Save changes
        await _stockMovementRepository.AddAsync(stockMovement);
        await _stockLevelRepository.UpdateAsync(stockLevel);

        // Add batch/serial information if available
        if (!string.IsNullOrEmpty(poItem.BatchNo))
        {
            // In a real implementation, you would set batch information on the stock movement
            _logger.LogDebug("Batch number {BatchNo} for item {ItemId}", poItem.BatchNo, poItem.ItemId);
        }

        if (poItem.ExpiryDate.HasValue)
        {
            // In a real implementation, you would set expiry date on the stock movement
            _logger.LogDebug("Expiry date {ExpiryDate} for item {ItemId}", poItem.ExpiryDate, poItem.ItemId);
        }

        _logger.LogInformation("Received {Quantity} units of item {ItemId} for PO {PurchaseOrderId}", 
            poItem.ReceivedQuantity, poItem.ItemId, poEvent.PurchaseOrderId);
    }

    private async Task<Guid?> GetDefaultWarehouseId()
    {
        // Get the first available warehouse as default
        var warehouses = await _warehouseRepository.GetAllAsync();
        return warehouses.FirstOrDefault()?.Id;
    }
}
