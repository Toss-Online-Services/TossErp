using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;

namespace Stock.Processor;

public class StockProcessorWorker : BackgroundService
{
    private readonly ILogger<StockProcessorWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly PeriodicTimer _timer;

    public StockProcessorWorker(
        ILogger<StockProcessorWorker> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        
        // Process stock operations every 5 minutes
        var interval = TimeSpan.FromMinutes(5);
        _timer = new PeriodicTimer(interval);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Stock Processor Worker started at: {time}", DateTimeOffset.Now);

        try
        {
            while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                await ProcessStockOperationsAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Stock Processor Worker was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in Stock Processor Worker");
        }
        finally
        {
            _timer.Dispose();
        }
    }

    private async Task ProcessStockOperationsAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing stock operations at: {time}", DateTimeOffset.Now);

            using var scope = _serviceProvider.CreateScope();
            
            // Process pending stock entries
            await ProcessPendingStockEntriesAsync(scope, cancellationToken);
            
            // Process stock reconciliations
            await ProcessStockReconciliationsAsync(scope, cancellationToken);
            
            // Process automated stock adjustments
            await ProcessAutomatedStockAdjustmentsAsync(scope, cancellationToken);

            _logger.LogInformation("Stock operations processing completed at: {time}", DateTimeOffset.Now);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing stock operations");
        }
    }

    private async Task ProcessPendingStockEntriesAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            var stockEntryRepository = scope.ServiceProvider.GetRequiredService<IStockEntryRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            // Get pending stock entries that are ready for processing
            var pendingEntries = await stockEntryRepository.GetPendingEntriesAsync(cancellationToken);

            foreach (var entry in pendingEntries)
            {
                try
                {
                    // Process each pending entry based on its type
                    switch (entry.EntryType)
                    {
                        case StockEntryType.Receipt:
                            await ProcessReceiptEntryAsync(entry, scope, cancellationToken);
                            break;
                        case StockEntryType.Issue:
                            await ProcessIssueEntryAsync(entry, scope, cancellationToken);
                            break;
                        case StockEntryType.Transfer:
                            await ProcessTransferEntryAsync(entry, scope, cancellationToken);
                            break;
                        case StockEntryType.Adjustment:
                            await ProcessAdjustmentEntryAsync(entry, scope, cancellationToken);
                            break;
                    }

                    // Mark entry as processed
                    entry.MarkAsProcessed();
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation("Processed stock entry {EntryId} of type {EntryType}", 
                        entry.Id, entry.EntryType);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing stock entry {EntryId}", entry.Id);
                    entry.MarkAsFailed(ex.Message);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing pending stock entries");
        }
    }

    private async Task ProcessReceiptEntryAsync(StockEntryAggregate entry, IServiceScope scope, CancellationToken cancellationToken)
    {
        var stockLevelRepository = scope.ServiceProvider.GetRequiredService<IStockLevelRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        foreach (var detail in entry.Details)
        {
            var stockLevel = await stockLevelRepository.GetByItemAndLocationAsync(
                detail.ItemId, detail.WarehouseId, detail.BinId, cancellationToken);

            if (stockLevel == null)
            {
                // Create new stock level if it doesn't exist
                stockLevel = new StockLevel(detail.ItemId, detail.WarehouseId, detail.BinId, 0, detail.UnitCost);
                await stockLevelRepository.AddAsync(stockLevel);
            }

            // Receive stock
            stockLevel.ReceiveStock(detail.Quantity, detail.UnitCost);
        }
    }

    private async Task ProcessIssueEntryAsync(StockEntryAggregate entry, IServiceScope scope, CancellationToken cancellationToken)
    {
        var stockLevelRepository = scope.ServiceProvider.GetRequiredService<IStockLevelRepository>();

        foreach (var detail in entry.Details)
        {
            var stockLevel = await stockLevelRepository.GetByItemAndLocationAsync(
                detail.ItemId, detail.WarehouseId, detail.BinId, cancellationToken);

            if (stockLevel != null && stockLevel.QuantityOnHand >= detail.Quantity)
            {
                stockLevel.IssueStock(detail.Quantity);
            }
            else
            {
                throw new InvalidOperationException($"Insufficient stock for item {detail.ItemId}");
            }
        }
    }

    private async Task ProcessTransferEntryAsync(StockEntryAggregate entry, IServiceScope scope, CancellationToken cancellationToken)
    {
        var stockLevelRepository = scope.ServiceProvider.GetRequiredService<IStockLevelRepository>();

        foreach (var detail in entry.Details)
        {
            var fromStockLevel = await stockLevelRepository.GetByItemAndLocationAsync(
                detail.ItemId, detail.FromWarehouseId, detail.FromBinId, cancellationToken);

            var toStockLevel = await stockLevelRepository.GetByItemAndLocationAsync(
                detail.ItemId, detail.WarehouseId, detail.BinId, cancellationToken);

            if (fromStockLevel != null && fromStockLevel.QuantityOnHand >= detail.Quantity)
            {
                // Issue from source
                fromStockLevel.IssueStock(detail.Quantity);

                // Receive at destination
                if (toStockLevel == null)
                {
                    toStockLevel = new StockLevel(detail.ItemId, detail.WarehouseId, detail.BinId, 0, detail.UnitCost);
                    await stockLevelRepository.AddAsync(toStockLevel);
                }

                toStockLevel.ReceiveStock(detail.Quantity, detail.UnitCost);
            }
            else
            {
                throw new InvalidOperationException($"Insufficient stock for transfer from location {detail.FromWarehouseId}");
            }
        }
    }

    private async Task ProcessAdjustmentEntryAsync(StockEntryAggregate entry, IServiceScope scope, CancellationToken cancellationToken)
    {
        var stockLevelRepository = scope.ServiceProvider.GetRequiredService<IStockLevelRepository>();

        foreach (var detail in entry.Details)
        {
            var stockLevel = await stockLevelRepository.GetByItemAndLocationAsync(
                detail.ItemId, detail.WarehouseId, detail.BinId, cancellationToken);

            if (stockLevel != null)
            {
                // Adjust stock to the specified quantity
                var difference = detail.Quantity - stockLevel.QuantityOnHand;
                if (difference > 0)
                {
                    stockLevel.ReceiveStock(difference, detail.UnitCost);
                }
                else if (difference < 0)
                {
                    stockLevel.IssueStock(Math.Abs(difference));
                }
            }
        }
    }

    private async Task ProcessStockReconciliationsAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            // Process stock reconciliations (e.g., stock takes, cycle counts)
            _logger.LogInformation("Processing stock reconciliations");
            
            // This would implement stock reconciliation logic
            // For now, just log that it's being processed
            await Task.Delay(100, cancellationToken); // Simulate work
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing stock reconciliations");
        }
    }

    private async Task ProcessAutomatedStockAdjustmentsAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            // Process automated stock adjustments based on business rules
            _logger.LogInformation("Processing automated stock adjustments");
            
            // This would implement automated adjustment logic
            // For now, just log that it's being processed
            await Task.Delay(100, cancellationToken); // Simulate work
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing automated stock adjustments");
        }
    }

    public override void Dispose()
    {
        _timer?.Dispose();
        base.Dispose();
    }
}
