using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eShop.EventBus.Abstractions;
using eShop.EventBus.Events.Sales;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;

namespace TossErp.Stock.Application.EventHandlers;

/// <summary>
/// Handles SaleCompleted integration events by updating stock levels
/// </summary>
public class SaleCompletedIntegrationEventHandler : IIntegrationEventHandler<SaleCompletedIntegrationEvent>
{
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SaleCompletedIntegrationEventHandler> _logger;

    public SaleCompletedIntegrationEventHandler(
        IStockLevelRepository stockLevelRepository,
        IStockMovementRepository stockMovementRepository,
        IItemRepository itemRepository,
        IWarehouseRepository warehouseRepository,
        IUnitOfWork unitOfWork,
        ILogger<SaleCompletedIntegrationEventHandler> logger)
    {
        _stockLevelRepository = stockLevelRepository;
        _stockMovementRepository = stockMovementRepository;
        _itemRepository = itemRepository;
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(SaleCompletedIntegrationEvent @event)
    {
        try
        {
            _logger.LogInformation("Processing SaleCompleted event for sale {SaleId}", @event.SaleId);

            // Process each item in the sale
            foreach (var saleItem in @event.Items)
            {
                await ProcessSaleItem(@event, saleItem);
            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Successfully processed SaleCompleted event for sale {SaleId}", @event.SaleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing SaleCompleted event for sale {SaleId}", @event.SaleId);
            throw;
        }
    }

    private async Task ProcessSaleItem(SaleCompletedIntegrationEvent saleEvent, SaleItemDto saleItem)
    {
        // Get the item to validate it exists
        var item = await _itemRepository.GetByIdAsync(saleItem.ItemId);
        if (item == null)
        {
            _logger.LogWarning("Item {ItemId} not found for sale {SaleId}", saleItem.ItemId, saleEvent.SaleId);
            return;
        }

        // Determine warehouse to use (from sale item or default)
        var warehouseId = saleItem.WarehouseId ?? await GetDefaultWarehouseId();
        if (warehouseId == null)
        {
            _logger.LogWarning("No warehouse specified for item {ItemId} in sale {SaleId}", saleItem.ItemId, saleEvent.SaleId);
            return;
        }

        // Get current stock level
        var stockLevel = await _stockLevelRepository.GetByItemAndWarehouseAsync(saleItem.ItemId, warehouseId.Value);
        if (stockLevel == null)
        {
            _logger.LogWarning("No stock level found for item {ItemId} in warehouse {WarehouseId}", saleItem.ItemId, warehouseId);
            return;
        }

        // Check if we have enough stock
        if (stockLevel.GetAvailableQuantity() < saleItem.Quantity)
        {
            _logger.LogWarning("Insufficient stock for item {ItemId}. Available: {Available}, Requested: {Requested}", 
                saleItem.ItemId, stockLevel.GetAvailableQuantity(), saleItem.Quantity);
            // In a real application, you might want to handle this differently
            // For now, we'll proceed with the available quantity
        }

        // Calculate quantity to issue (don't go below zero)
        var quantityToIssue = Math.Min(saleItem.Quantity, stockLevel.GetAvailableQuantity());
        if (quantityToIssue <= 0)
        {
            _logger.LogWarning("No stock available to issue for item {ItemId}", saleItem.ItemId);
            return;
        }

        // Create stock movement using the static factory method
        var stockMovement = StockMovement.CreateIssue(
            Guid.NewGuid(), // TenantId - should come from context
            saleItem.ItemId,
            warehouseId.Value,
            quantityToIssue,
            saleEvent.CompletedBy ?? "System", // CreatedBy
            saleItem.BinId, // BinId
            saleItem.UnitPrice, // UnitCost
            saleEvent.SaleId.ToString(), // ReferenceNumber
            "Sale", // ReferenceType
            $"Sale {saleEvent.SaleNumber}" // Reason
        );

        // Update stock level
        stockLevel.IssueStock(quantityToIssue);

        // Save changes
        await _stockMovementRepository.AddAsync(stockMovement);
        await _stockLevelRepository.UpdateAsync(stockLevel);

        _logger.LogInformation("Issued {Quantity} units of item {ItemId} for sale {SaleId}", 
            quantityToIssue, saleItem.ItemId, saleEvent.SaleId);
    }

    private async Task<Guid?> GetDefaultWarehouseId()
    {
        // Get the first available warehouse as default
        var warehouses = await _warehouseRepository.GetAllAsync();
        return warehouses.FirstOrDefault()?.Id;
    }
}
