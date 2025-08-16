using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Entities;

namespace Stock.Processor;

public class LowStockAlertWorker : BackgroundService
{
    private readonly ILogger<LowStockAlertWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly PeriodicTimer _timer;

    public LowStockAlertWorker(
        ILogger<LowStockAlertWorker> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        
        // Check stock levels every 15 minutes
        var interval = TimeSpan.FromMinutes(15);
        _timer = new PeriodicTimer(interval);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Low Stock Alert Worker started at: {time}", DateTimeOffset.Now);

        try
        {
            while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                await CheckLowStockLevelsAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Low Stock Alert Worker was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in Low Stock Alert Worker");
        }
        finally
        {
            _timer.Dispose();
        }
    }

    private async Task CheckLowStockLevelsAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Checking low stock levels at: {time}", DateTimeOffset.Now);

            using var scope = _serviceProvider.CreateScope();
            
            var itemRepository = scope.ServiceProvider.GetRequiredService<IItemRepository>();
            var stockLevelRepository = scope.ServiceProvider.GetRequiredService<IStockLevelRepository>();

            // Get all active stock items
            var stockItems = await itemRepository.GetStockItemsAsync(cancellationToken);

            foreach (var item in stockItems)
            {
                try
                {
                    await CheckItemStockLevelAsync(item, stockLevelRepository, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking stock level for item {ItemId}", item.Id);
                }
            }

            _logger.LogInformation("Low stock level check completed at: {time}", DateTimeOffset.Now);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking low stock levels");
        }
    }

    private async Task CheckItemStockLevelAsync(
        ItemAggregate item, 
        IStockLevelRepository stockLevelRepository, 
        CancellationToken cancellationToken)
    {
        // Get total stock across all warehouses for this item
        var totalStock = await stockLevelRepository.GetTotalStockByItemAsync(item.Id, cancellationToken);

        if (totalStock < item.ReorderLevel)
        {
            // Log low stock alert
            _logger.LogWarning("Low stock alert for item {ItemName} (ID: {ItemId}). Current stock: {CurrentStock}, Reorder level: {ReorderLevel}", 
                item.Name, item.Id, totalStock, item.ReorderLevel);

            // Send notification (this would integrate with your notification service)
            await SendLowStockNotificationAsync(item, totalStock, cancellationToken);

            // Create reorder suggestion
            await CreateReorderSuggestionAsync(item, totalStock, cancellationToken);
        }
    }

    private async Task SendLowStockNotificationAsync(
        ItemAggregate item, 
        decimal currentStock, 
        CancellationToken cancellationToken)
    {
        try
        {
            // This would integrate with your notification service
            // For now, just log the notification
            _logger.LogInformation("Low stock notification sent for item {ItemName}. Current stock: {CurrentStock}, Reorder level: {ReorderLevel}", 
                item.Name, currentStock, item.ReorderLevel);

            // TODO: Integrate with notification service
            // var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            // await notificationService.SendLowStockAlertAsync(item, currentStock, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending low stock notification for item {ItemId}", item.Id);
        }
    }

    private async Task CreateReorderSuggestionAsync(
        ItemAggregate item, 
        decimal currentStock, 
        CancellationToken cancellationToken)
    {
        try
        {
            // This would create a reorder suggestion in your system
            // For now, just log the suggestion
            var suggestedQuantity = item.ReorderQuantity;
            
            _logger.LogInformation("Reorder suggestion created for item {ItemName}. Suggested quantity: {SuggestedQuantity}", 
                item.Name, suggestedQuantity);

            // TODO: Integrate with purchasing/reordering system
            // var purchasingService = scope.ServiceProvider.GetRequiredService<IPurchasingService>();
            // await purchasingService.CreateReorderSuggestionAsync(item, suggestedQuantity, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating reorder suggestion for item {ItemId}", item.Id);
        }
    }

    public override void Dispose()
    {
        _timer?.Dispose();
        base.Dispose();
    }
}
