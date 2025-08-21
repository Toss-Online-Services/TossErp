namespace TossErp.Assets.Infrastructure.BackgroundServices;

/// <summary>
/// Background service for calculating asset depreciation
/// </summary>
public class AssetDepreciationCalculationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AssetDepreciationCalculationService> _logger;
    private readonly TimeSpan _calculationInterval = TimeSpan.FromDays(1); // Calculate daily

    public AssetDepreciationCalculationService(
        IServiceProvider serviceProvider,
        ILogger<AssetDepreciationCalculationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Asset Depreciation Calculation Service started");

        // Wait for a random delay to avoid all services starting at the same time
        var initialDelay = TimeSpan.FromMinutes(Random.Shared.Next(1, 30));
        await Task.Delay(initialDelay, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CalculateDepreciation(stoppingToken);
                await Task.Delay(_calculationInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating asset depreciation");
                
                // Wait before retrying to avoid tight error loops
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        _logger.LogInformation("Asset Depreciation Calculation Service stopped");
    }

    private async Task CalculateDepreciation(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AssetsDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Get assets that need depreciation calculation
        var assetsToProcess = await context.Assets
            .Where(a => a.FinancialInfo.PurchasePrice > 0 &&
                       a.FinancialInfo.UsefulLifeYears > 0 &&
                       a.Status != AssetStatus.Disposed &&
                       (a.FinancialInfo.LastDepreciationDate == null || 
                        a.FinancialInfo.LastDepreciationDate < DateTime.UtcNow.Date))
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Processing depreciation for {Count} assets", assetsToProcess.Count);

        var batchSize = 100;
        var processedCount = 0;

        for (int i = 0; i < assetsToProcess.Count; i += batchSize)
        {
            var batch = assetsToProcess.Skip(i).Take(batchSize);
            
            await ProcessDepreciationBatch(batch, context, mediator, cancellationToken);
            processedCount += batch.Count();
            
            _logger.LogInformation("Processed depreciation for {ProcessedCount}/{TotalCount} assets", 
                processedCount, assetsToProcess.Count);

            // Small delay between batches to avoid overwhelming the database
            await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
        }

        if (assetsToProcess.Any())
        {
            await context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Completed depreciation calculation for {Count} assets", assetsToProcess.Count);
        }
    }

    private async Task ProcessDepreciationBatch(
        IEnumerable<Asset> assets, 
        AssetsDbContext context,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        foreach (var asset in assets)
        {
            try
            {
                var depreciation = CalculateAssetDepreciation(asset);
                
                if (depreciation.Amount > 0)
                {
                    // Update asset financial information
                    asset.FinancialInfo.AccumulatedDepreciation += depreciation.Amount;
                    asset.FinancialInfo.CurrentValue = Math.Max(0, 
                        asset.FinancialInfo.PurchasePrice - asset.FinancialInfo.AccumulatedDepreciation);
                    asset.FinancialInfo.LastDepreciationDate = DateTime.UtcNow.Date;
                    asset.UpdatedAt = DateTime.UtcNow;
                    asset.UpdatedBy = "System";

                    // Publish depreciation event
                    var depreciationEvent = new AssetDepreciationCalculatedEvent
                    {
                        TenantId = asset.TenantId,
                        AssetId = asset.Id,
                        AssetTag = asset.AssetTag,
                        AssetName = asset.Name,
                        DepreciationAmount = depreciation.Amount,
                        AccumulatedDepreciation = asset.FinancialInfo.AccumulatedDepreciation,
                        CurrentValue = asset.FinancialInfo.CurrentValue,
                        DepreciationMethod = depreciation.Method,
                        CalculationDate = DateTime.UtcNow
                    };

                    await mediator.Publish(depreciationEvent, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to calculate depreciation for asset {AssetId} ({AssetTag})", 
                    asset.Id, asset.AssetTag);
            }
        }
    }

    private DepreciationCalculation CalculateAssetDepreciation(Asset asset)
    {
        var purchaseDate = asset.PurchaseDate ?? asset.CreatedAt.Date;
        var daysSincePurchase = (DateTime.UtcNow.Date - purchaseDate).Days;
        var lastCalculationDate = asset.FinancialInfo.LastDepreciationDate ?? purchaseDate;
        var daysSinceLastCalculation = (DateTime.UtcNow.Date - lastCalculationDate).Days;

        // Only calculate if at least one day has passed since last calculation
        if (daysSinceLastCalculation == 0)
        {
            return new DepreciationCalculation { Amount = 0, Method = "None" };
        }

        var method = asset.FinancialInfo.DepreciationMethod?.ToLowerInvariant() ?? "straight-line";
        
        return method switch
        {
            "straight-line" => CalculateStraightLineDepreciation(asset, daysSinceLastCalculation),
            "declining-balance" => CalculateDecliningBalanceDepreciation(asset, daysSinceLastCalculation),
            "units-of-production" => CalculateUnitsOfProductionDepreciation(asset, daysSinceLastCalculation),
            _ => CalculateStraightLineDepreciation(asset, daysSinceLastCalculation)
        };
    }

    private DepreciationCalculation CalculateStraightLineDepreciation(Asset asset, int daysSinceLastCalculation)
    {
        var depreciableAmount = asset.FinancialInfo.PurchasePrice - (asset.FinancialInfo.SalvageValue ?? 0);
        var totalDays = asset.FinancialInfo.UsefulLifeYears * 365;
        var dailyDepreciation = depreciableAmount / totalDays;
        var amount = dailyDepreciation * daysSinceLastCalculation;

        // Ensure we don't depreciate below salvage value
        var maxDepreciation = depreciableAmount - asset.FinancialInfo.AccumulatedDepreciation;
        amount = Math.Min(amount, maxDepreciation);

        return new DepreciationCalculation 
        { 
            Amount = Math.Max(0, amount), 
            Method = "Straight-Line" 
        };
    }

    private DepreciationCalculation CalculateDecliningBalanceDepreciation(Asset asset, int daysSinceLastCalculation)
    {
        var rate = asset.FinancialInfo.DepreciationRate ?? (2.0m / asset.FinancialInfo.UsefulLifeYears); // Double declining balance
        var bookValue = asset.FinancialInfo.PurchasePrice - asset.FinancialInfo.AccumulatedDepreciation;
        var salvageValue = asset.FinancialInfo.SalvageValue ?? 0;
        
        var annualDepreciation = bookValue * rate;
        var dailyDepreciation = annualDepreciation / 365;
        var amount = dailyDepreciation * daysSinceLastCalculation;

        // Don't depreciate below salvage value
        var maxDepreciation = Math.Max(0, bookValue - salvageValue);
        amount = Math.Min(amount, maxDepreciation);

        return new DepreciationCalculation 
        { 
            Amount = Math.Max(0, amount), 
            Method = "Declining-Balance" 
        };
    }

    private DepreciationCalculation CalculateUnitsOfProductionDepreciation(Asset asset, int daysSinceLastCalculation)
    {
        // This would require tracking actual usage units
        // For now, fall back to straight-line
        return CalculateStraightLineDepreciation(asset, daysSinceLastCalculation);
    }
}

/// <summary>
/// Depreciation calculation result
/// </summary>
public class DepreciationCalculation
{
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
}

/// <summary>
/// Event published when asset depreciation is calculated
/// </summary>
public class AssetDepreciationCalculatedEvent : INotification
{
    public Guid TenantId { get; set; }
    public Guid AssetId { get; set; }
    public string AssetTag { get; set; } = string.Empty;
    public string AssetName { get; set; } = string.Empty;
    public decimal DepreciationAmount { get; set; }
    public decimal AccumulatedDepreciation { get; set; }
    public decimal CurrentValue { get; set; }
    public string DepreciationMethod { get; set; } = string.Empty;
    public DateTime CalculationDate { get; set; }
}
