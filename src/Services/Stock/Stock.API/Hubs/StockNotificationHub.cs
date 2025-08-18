using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using TossErp.Stock.Application.Common.Interfaces;

namespace TossErp.Stock.API.Hubs;

/// <summary>
/// SignalR Hub for real-time stock notifications
/// </summary>
[Authorize]
public class StockNotificationHub : Hub
{
    private readonly ILogger<StockNotificationHub> _logger;
    private readonly ICurrentUser _currentUser;

    public StockNotificationHub(
        ILogger<StockNotificationHub> logger,
        ICurrentUser currentUser)
    {
        _logger = logger;
        _currentUser = currentUser;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = _currentUser.UserId;
        var tenantId = _currentUser.TenantId;
        
        _logger.LogInformation("User {UserId} from tenant {TenantId} connected to Stock Notification Hub", 
            userId, tenantId);

        // Join tenant-specific group for notifications
        if (!string.IsNullOrEmpty(tenantId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant-{tenantId}");
            _logger.LogDebug("Added connection {ConnectionId} to tenant group {TenantId}", 
                Context.ConnectionId, tenantId);
        }

        // Join user-specific group for personal notifications
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
            _logger.LogDebug("Added connection {ConnectionId} to user group {UserId}", 
                Context.ConnectionId, userId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = _currentUser.UserId;
        var tenantId = _currentUser.TenantId;
        
        _logger.LogInformation("User {UserId} from tenant {TenantId} disconnected from Stock Notification Hub", 
            userId, tenantId);

        if (exception != null)
        {
            _logger.LogError(exception, "User {UserId} disconnected with error", userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Subscribe to specific item notifications
    /// </summary>
    /// <param name="itemId">Item ID to subscribe to</param>
    public async Task SubscribeToItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            _logger.LogWarning("Invalid item ID provided for subscription");
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"item-{itemId}");
        _logger.LogDebug("Connection {ConnectionId} subscribed to item {ItemId}", 
            Context.ConnectionId, itemId);
        
        await Clients.Caller.SendAsync("SubscriptionConfirmed", itemId);
    }

    /// <summary>
    /// Unsubscribe from specific item notifications
    /// </summary>
    /// <param name="itemId">Item ID to unsubscribe from</param>
    public async Task UnsubscribeFromItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            _logger.LogWarning("Invalid item ID provided for unsubscription");
            return;
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"item-{itemId}");
        _logger.LogDebug("Connection {ConnectionId} unsubscribed from item {ItemId}", 
            Context.ConnectionId, itemId);
        
        await Clients.Caller.SendAsync("UnsubscriptionConfirmed", itemId);
    }

    /// <summary>
    /// Subscribe to warehouse-specific notifications
    /// </summary>
    /// <param name="warehouseId">Warehouse ID to subscribe to</param>
    public async Task SubscribeToWarehouse(string warehouseId)
    {
        if (string.IsNullOrEmpty(warehouseId))
        {
            _logger.LogWarning("Invalid warehouse ID provided for subscription");
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"warehouse-{warehouseId}");
        _logger.LogDebug("Connection {ConnectionId} subscribed to warehouse {WarehouseId}", 
            Context.ConnectionId, warehouseId);
        
        await Clients.Caller.SendAsync("WarehouseSubscriptionConfirmed", warehouseId);
    }

    /// <summary>
    /// Unsubscribe from warehouse-specific notifications
    /// </summary>
    /// <param name="warehouseId">Warehouse ID to unsubscribe from</param>
    public async Task UnsubscribeFromWarehouse(string warehouseId)
    {
        if (string.IsNullOrEmpty(warehouseId))
        {
            _logger.LogWarning("Invalid warehouse ID provided for unsubscription");
            return;
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"warehouse-{warehouseId}");
        _logger.LogDebug("Connection {ConnectionId} unsubscribed from warehouse {WarehouseId}", 
            Context.ConnectionId, warehouseId);
        
        await Clients.Caller.SendAsync("WarehouseUnsubscriptionConfirmed", warehouseId);
    }

    /// <summary>
    /// Get current connection information
    /// </summary>
    public async Task GetConnectionInfo()
    {
        var connectionInfo = new
        {
            ConnectionId = Context.ConnectionId,
            UserId = _currentUser.UserId,
            TenantId = _currentUser.TenantId,
            ConnectedAt = DateTime.UtcNow
        };

        await Clients.Caller.SendAsync("ConnectionInfo", connectionInfo);
    }
}
