using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;

namespace Stock.Processor;

public class StockReconciliationWorker : BackgroundService
{
    private readonly ILogger<StockReconciliationWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly PeriodicTimer _timer;

    public StockReconciliationWorker(
        ILogger<StockReconciliationWorker> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        
        // Run reconciliation every 6 hours
        var interval = TimeSpan.FromHours(6);
        _timer = new PeriodicTimer(interval);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Stock Reconciliation Worker started at: {time}", DateTimeOffset.Now);

        try
        {
            while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                await RunStockReconciliationAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Stock Reconciliation Worker was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in Stock Reconciliation Worker");
        }
        finally
        {
            _timer.Dispose();
        }
    }

    private async Task RunStockReconciliationAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting stock reconciliation at: {time}", DateTimeOffset.Now);

            using var scope = _serviceProvider.CreateScope();
            
            // Run different types of reconciliations
            await RunCycleCountReconciliationAsync(scope, cancellationToken);
            await RunABCReconciliationAsync(scope, cancellationToken);
            await RunDeadStockReconciliationAsync(scope, cancellationToken);

            _logger.LogInformation("Stock reconciliation completed at: {time}", DateTimeOffset.Now);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during stock reconciliation");
        }
    }

    private async Task RunCycleCountReconciliationAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Running cycle count reconciliation");

            var itemRepository = scope.ServiceProvider.GetRequiredService<IItemRepository>();
            var stockLevelRepository = scope.ServiceProvider.GetRequiredService<IStockLevelRepository>();

            // Get items that are due for cycle counting
            var itemsDueForCycleCount = await GetItemsDueForCycleCountAsync(itemRepository, cancellationToken);

            foreach (var item in itemsDueForCycleCount)
            {
                try
                {
                    await ProcessCycleCountForItemAsync(item, stockLevelRepository, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing cycle count for item {ItemId}", item.Id);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running cycle count reconciliation");
        }
    }

    private async Task<List<ItemAggregate>> GetItemsDueForCycleCountAsync(
        IItemRepository itemRepository, 
        CancellationToken cancellationToken)
    {
        // This would implement logic to determine which items need cycle counting
        // For now, return an empty list
        return new List<ItemAggregate>();
    }

    private async Task ProcessCycleCountForItemAsync(
        ItemAggregate item, 
        IStockLevelRepository stockLevelRepository, 
        CancellationToken cancellationToken)
    {
        // This would implement the actual cycle count process
        // For now, just log that it's being processed
        _logger.LogInformation("Processing cycle count for item {ItemName}", item.Name);
        
        // TODO: Implement cycle count logic
        // 1. Get current system stock levels
        // 2. Compare with physical counts
        // 3. Create adjustment entries for discrepancies
        // 4. Update stock levels
    }

    private async Task RunABCReconciliationAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Running ABC analysis reconciliation");

            var itemRepository = scope.ServiceProvider.GetRequiredService<IItemRepository>();
            var stockLevelRepository = scope.ServiceProvider.GetRequiredService<IStockLevelRepository>();

            // Get all stock items for ABC analysis
            var stockItems = await itemRepository.GetStockItemsAsync(cancellationToken);

            // Perform ABC analysis
            var abcResults = await PerformABCAnalysisAsync(stockItems, stockLevelRepository, cancellationToken);

            // Apply ABC recommendations
            await ApplyABCRecommendationsAsync(abcResults, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running ABC reconciliation");
        }
    }

    private async Task<List<ABCResult>> PerformABCAnalysisAsync(
        List<ItemAggregate> items, 
        IStockLevelRepository stockLevelRepository, 
        CancellationToken cancellationToken)
    {
        var results = new List<ABCResult>();

        foreach (var item in items)
        {
            try
            {
                var totalStock = await stockLevelRepository.GetTotalStockByItemAsync(item.Id, cancellationToken);
                var totalValue = totalStock * item.DefaultPrice;

                var abcCategory = DetermineABCCategory(totalValue);
                
                results.Add(new ABCResult
                {
                    ItemId = item.Id,
                    ItemName = item.Name,
                    TotalStock = totalStock,
                    TotalValue = totalValue,
                    Category = abcCategory,
                    RecommendedAction = GetRecommendedAction(abcCategory)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing ABC analysis for item {ItemId}", item.Id);
            }
        }

        return results;
    }

    private ABCCategory DetermineABCCategory(decimal totalValue)
    {
        // Simple ABC categorization based on value
        if (totalValue >= 10000m) return ABCCategory.A;
        if (totalValue >= 1000m) return ABCCategory.B;
        return ABCCategory.C;
    }

    private string GetRecommendedAction(ABCCategory category)
    {
        return category switch
        {
            ABCCategory.A => "Frequent cycle counting, tight control",
            ABCCategory.B => "Moderate cycle counting, standard control",
            ABCCategory.C => "Annual cycle counting, loose control",
            _ => "Unknown category"
        };
    }

    private async Task ApplyABCRecommendationsAsync(List<ABCResult> results, CancellationToken cancellationToken)
    {
        foreach (var result in results)
        {
            try
            {
                _logger.LogInformation("ABC recommendation for {ItemName}: {Action}", 
                    result.ItemName, result.RecommendedAction);

                // TODO: Apply ABC recommendations
                // This could include:
                // - Adjusting cycle count frequencies
                // - Setting different reorder levels
                // - Adjusting safety stock levels
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying ABC recommendation for item {ItemId}", result.ItemId);
            }
        }
    }

    private async Task RunDeadStockReconciliationAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Running dead stock reconciliation");

            var itemRepository = scope.ServiceProvider.GetRequiredService<IItemRepository>();
            var stockLevelRepository = scope.ServiceProvider.GetRequiredService<IStockLevelRepository>();

            // Identify dead stock (items with no movement for extended periods)
            var deadStockItems = await IdentifyDeadStockAsync(itemRepository, stockLevelRepository, cancellationToken);

            foreach (var item in deadStockItems)
            {
                try
                {
                    await ProcessDeadStockItemAsync(item, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing dead stock item {ItemId}", item.Id);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running dead stock reconciliation");
        }
    }

    private async Task<List<ItemAggregate>> IdentifyDeadStockAsync(
        IItemRepository itemRepository, 
        IStockLevelRepository stockLevelRepository, 
        CancellationToken cancellationToken)
    {
        // This would implement logic to identify dead stock
        // For now, return an empty list
        return new List<ItemAggregate>();
    }

    private async Task ProcessDeadStockItemAsync(ItemAggregate item, CancellationToken cancellationToken)
    {
        // This would implement dead stock processing
        // For now, just log that it's being processed
        _logger.LogInformation("Processing dead stock item {ItemName}", item.Name);
        
        // TODO: Implement dead stock processing
        // This could include:
        // - Marking items for disposal
        // - Creating write-off entries
        // - Adjusting stock levels to zero
    }

    public override void Dispose()
    {
        _timer?.Dispose();
        base.Dispose();
    }
}

public class ABCResult
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal TotalStock { get; set; }
    public decimal TotalValue { get; set; }
    public ABCCategory Category { get; set; }
    public string RecommendedAction { get; set; } = string.Empty;
}

public enum ABCCategory
{
    A,
    B,
    C
}
