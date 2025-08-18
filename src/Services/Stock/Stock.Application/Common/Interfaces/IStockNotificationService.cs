using TossErp.Stock.Domain.Entities;

namespace TossErp.Stock.Application.Common.Interfaces;

/// <summary>
/// Interface for sending real-time stock notifications
/// </summary>
public interface IStockNotificationService
{
    /// <summary>
    /// Send low stock alert notification
    /// </summary>
    /// <param name="item">The item with low stock</param>
    /// <param name="currentStock">Current stock level</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendLowStockAlertAsync(
        ItemAggregate item, 
        decimal currentStock, 
        string tenantId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send stock level updated notification
    /// </summary>
    /// <param name="itemId">Item ID</param>
    /// <param name="itemName">Item name</param>
    /// <param name="itemSku">Item SKU</param>
    /// <param name="warehouseId">Warehouse ID</param>
    /// <param name="warehouseName">Warehouse name</param>
    /// <param name="previousLevel">Previous stock level</param>
    /// <param name="newLevel">New stock level</param>
    /// <param name="movementType">Type of movement</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendStockLevelUpdatedAsync(
        Guid itemId, 
        string itemName, 
        string itemSku,
        Guid? warehouseId,
        string warehouseName,
        decimal previousLevel, 
        decimal newLevel, 
        string movementType,
        string tenantId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send reorder suggestion notification
    /// </summary>
    /// <param name="item">The item to reorder</param>
    /// <param name="suggestedQuantity">Suggested quantity to order</param>
    /// <param name="estimatedCost">Estimated cost of the order</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendReorderSuggestionAsync(
        ItemAggregate item, 
        decimal suggestedQuantity, 
        decimal estimatedCost,
        string tenantId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send stock movement notification
    /// </summary>
    /// <param name="itemId">Item ID</param>
    /// <param name="itemName">Item name</param>
    /// <param name="movementType">Type of movement</param>
    /// <param name="quantity">Quantity moved</param>
    /// <param name="fromWarehouseId">Source warehouse ID</param>
    /// <param name="fromWarehouseName">Source warehouse name</param>
    /// <param name="toWarehouseId">Destination warehouse ID</param>
    /// <param name="toWarehouseName">Destination warehouse name</param>
    /// <param name="reference">Movement reference</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendStockMovementAsync(
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
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send batch operation completion notification
    /// </summary>
    /// <param name="operationType">Type of operation</param>
    /// <param name="totalItems">Total number of items processed</param>
    /// <param name="successfulItems">Number of successfully processed items</param>
    /// <param name="failedItems">Number of failed items</param>
    /// <param name="duration">Duration of the operation</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendBatchOperationCompleteAsync(
        string operationType,
        int totalItems,
        int successfulItems,
        int failedItems,
        TimeSpan duration,
        string tenantId,
        CancellationToken cancellationToken = default);
}
