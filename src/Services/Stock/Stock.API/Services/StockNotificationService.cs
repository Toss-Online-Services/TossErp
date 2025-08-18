using Microsoft.AspNetCore.SignalR;
using TossErp.Stock.API.Hubs;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Entities;

namespace TossErp.Stock.API.Services;

/// <summary>
/// Service for sending real-time stock notifications via SignalR
/// </summary>
public class StockNotificationService : IStockNotificationService
{
    private readonly IHubContext<StockNotificationHub> _hubContext;
    private readonly ILogger<StockNotificationService> _logger;

    public StockNotificationService(
        IHubContext<StockNotificationHub> hubContext,
        ILogger<StockNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <summary>
    /// Send low stock alert notification
    /// </summary>
    public async Task SendLowStockAlertAsync(
        ItemAggregate item, 
        decimal currentStock, 
        string tenantId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = new
            {
                Type = "LowStockAlert",
                ItemId = item.Id,
                ItemName = item.Name,
                ItemSku = item.SKU,
                CurrentStock = currentStock,
                ReorderLevel = item.ReorderLevel,
                ReorderQuantity = item.ReorderQuantity,
                Severity = GetAlertSeverity(currentStock, item.ReorderLevel),
                Timestamp = DateTime.UtcNow,
                TenantId = tenantId
            };

            // Send to tenant group
            await _hubContext.Clients.Group($"tenant-{tenantId}")
                .SendAsync("LowStockAlert", notification, cancellationToken);

            // Send to item-specific subscribers
            await _hubContext.Clients.Group($"item-{item.Id}")
                .SendAsync("ItemLowStock", notification, cancellationToken);

            _logger.LogInformation("Low stock alert sent for item {ItemName} (ID: {ItemId}) to tenant {TenantId}", 
                item.Name, item.Id, tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send low stock alert for item {ItemId}", item.Id);
            throw;
        }
    }

    /// <summary>
    /// Send stock level updated notification
    /// </summary>
    public async Task SendStockLevelUpdatedAsync(
        Guid itemId, 
        string itemName, 
        string itemSku,
        Guid? warehouseId,
        string warehouseName,
        decimal previousLevel, 
        decimal newLevel, 
        string movementType,
        string tenantId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = new
            {
                Type = "StockLevelUpdated",
                ItemId = itemId,
                ItemName = itemName,
                ItemSku = itemSku,
                WarehouseId = warehouseId,
                WarehouseName = warehouseName,
                PreviousLevel = previousLevel,
                NewLevel = newLevel,
                MovementType = movementType,
                Change = newLevel - previousLevel,
                Timestamp = DateTime.UtcNow,
                TenantId = tenantId
            };

            // Send to tenant group
            await _hubContext.Clients.Group($"tenant-{tenantId}")
                .SendAsync("StockLevelUpdated", notification, cancellationToken);

            // Send to item-specific subscribers
            await _hubContext.Clients.Group($"item-{itemId}")
                .SendAsync("ItemStockUpdated", notification, cancellationToken);

            // Send to warehouse-specific subscribers if applicable
            if (warehouseId.HasValue)
            {
                await _hubContext.Clients.Group($"warehouse-{warehouseId}")
                    .SendAsync("WarehouseStockUpdated", notification, cancellationToken);
            }

            _logger.LogDebug("Stock level update notification sent for item {ItemName} in warehouse {WarehouseName}", 
                itemName, warehouseName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send stock level update notification for item {ItemId}", itemId);
            throw;
        }
    }

    /// <summary>
    /// Send reorder suggestion notification
    /// </summary>
    public async Task SendReorderSuggestionAsync(
        ItemAggregate item, 
        decimal suggestedQuantity, 
        decimal estimatedCost,
        string tenantId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = new
            {
                Type = "ReorderSuggestion",
                ItemId = item.Id,
                ItemName = item.Name,
                ItemSku = item.SKU,
                SuggestedQuantity = suggestedQuantity,
                EstimatedCost = estimatedCost,
                ReorderLevel = item.ReorderLevel,
                CurrentStock = 0m, // This would be fetched from stock level service
                Timestamp = DateTime.UtcNow,
                TenantId = tenantId
            };

            // Send to tenant group
            await _hubContext.Clients.Group($"tenant-{tenantId}")
                .SendAsync("ReorderSuggestion", notification, cancellationToken);

            // Send to item-specific subscribers
            await _hubContext.Clients.Group($"item-{item.Id}")
                .SendAsync("ItemReorderSuggestion", notification, cancellationToken);

            _logger.LogInformation("Reorder suggestion sent for item {ItemName} (ID: {ItemId}) to tenant {TenantId}", 
                item.Name, item.Id, tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send reorder suggestion for item {ItemId}", item.Id);
            throw;
        }
    }

    /// <summary>
    /// Send stock movement notification
    /// </summary>
    public async Task SendStockMovementAsync(
        Guid itemId,
        string itemName,
        string movementType,
        decimal quantity,
        Guid? fromWarehouseId,
        string fromWarehouseName,
        Guid? toWarehouseId,
        string toWarehouseName,
        string reference,
        string tenantId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = new
            {
                Type = "StockMovement",
                ItemId = itemId,
                ItemName = itemName,
                MovementType = movementType,
                Quantity = quantity,
                FromWarehouseId = fromWarehouseId,
                FromWarehouseName = fromWarehouseName,
                ToWarehouseId = toWarehouseId,
                ToWarehouseName = toWarehouseName,
                Reference = reference,
                Timestamp = DateTime.UtcNow,
                TenantId = tenantId
            };

            // Send to tenant group
            await _hubContext.Clients.Group($"tenant-{tenantId}")
                .SendAsync("StockMovement", notification, cancellationToken);

            // Send to item-specific subscribers
            await _hubContext.Clients.Group($"item-{itemId}")
                .SendAsync("ItemMovement", notification, cancellationToken);

            // Send to warehouse-specific subscribers
            if (fromWarehouseId.HasValue)
            {
                await _hubContext.Clients.Group($"warehouse-{fromWarehouseId}")
                    .SendAsync("WarehouseMovement", notification, cancellationToken);
            }

            if (toWarehouseId.HasValue && toWarehouseId != fromWarehouseId)
            {
                await _hubContext.Clients.Group($"warehouse-{toWarehouseId}")
                    .SendAsync("WarehouseMovement", notification, cancellationToken);
            }

            _logger.LogDebug("Stock movement notification sent for item {ItemName}: {MovementType} {Quantity}", 
                itemName, movementType, quantity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send stock movement notification for item {ItemId}", itemId);
            throw;
        }
    }

    /// <summary>
    /// Send batch operation completion notification
    /// </summary>
    public async Task SendBatchOperationCompleteAsync(
        string operationType,
        int totalItems,
        int successfulItems,
        int failedItems,
        TimeSpan duration,
        string tenantId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = new
            {
                Type = "BatchOperationComplete",
                OperationType = operationType,
                TotalItems = totalItems,
                SuccessfulItems = successfulItems,
                FailedItems = failedItems,
                Duration = duration.TotalSeconds,
                SuccessRate = totalItems > 0 ? (double)successfulItems / totalItems * 100 : 0,
                Timestamp = DateTime.UtcNow,
                TenantId = tenantId
            };

            // Send to tenant group
            await _hubContext.Clients.Group($"tenant-{tenantId}")
                .SendAsync("BatchOperationComplete", notification, cancellationToken);

            _logger.LogInformation("Batch operation notification sent: {OperationType} completed for tenant {TenantId}", 
                operationType, tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send batch operation notification for tenant {TenantId}", tenantId);
            throw;
        }
    }

    /// <summary>
    /// Determine alert severity based on stock levels
    /// </summary>
    private static string GetAlertSeverity(decimal currentStock, decimal reorderLevel)
    {
        if (currentStock <= 0)
            return "Critical"; // Out of stock
        
        var percentage = currentStock / reorderLevel;
        
        return percentage switch
        {
            <= 0.25m => "Critical", // 25% or less of reorder level
            <= 0.5m => "High",      // 50% or less of reorder level
            <= 0.75m => "Medium",   // 75% or less of reorder level
            _ => "Low"              // Above 75% of reorder level
        };
    }
}
