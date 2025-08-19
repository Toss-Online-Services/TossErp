using Microsoft.Extensions.Logging;
using TossErp.Accounting.Application.Common.Interfaces;
using TossErp.Accounting.Domain.Common;
using TossErp.Accounting.Domain.Entities;
using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Application.Services;

/// <summary>
/// Implementation of stock valuation service for P&L reporting
/// </summary>
public class StockValuationService : IStockValuationService
{
    private readonly IStockValuationSnapshotRepository _snapshotRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StockValuationService> _logger;

    public StockValuationService(
        IStockValuationSnapshotRepository snapshotRepository,
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        ILogger<StockValuationService> logger)
    {
        _snapshotRepository = snapshotRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Money> CalculateStockValuationAsync(DateTime valuationDate, ValuationMethod method, string tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Calculating stock valuation for {Date} using {Method}", valuationDate, method);

            // For MVP, we'll use a simplified calculation
            // In a real implementation, this would integrate with the Inventory service
            var snapshots = await _snapshotRepository.GetByDateAsync(valuationDate, tenantId, cancellationToken);
            
            var totalValue = snapshots
                .Where(s => s.Method == method)
                .Sum(s => s.TotalValue.Amount);

            return new Money(totalValue, "ZAR");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating stock valuation for {Date}", valuationDate);
            throw;
        }
    }

    public async Task<Money> CalculateWarehouseValuationAsync(string warehouseCode, DateTime valuationDate, ValuationMethod method, string tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Calculating warehouse valuation for {Warehouse} on {Date}", warehouseCode, valuationDate);

            var snapshots = await _snapshotRepository.GetByWarehouseAsync(warehouseCode, tenantId, cancellationToken);
            
            var totalValue = snapshots
                .Where(s => s.SnapshotDate.Date == valuationDate.Date && s.Method == method)
                .Sum(s => s.TotalValue.Amount);

            return new Money(totalValue, "ZAR");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating warehouse valuation for {Warehouse}", warehouseCode);
            throw;
        }
    }

    public async Task<Money> CalculateItemValuationAsync(string itemCode, DateTime valuationDate, ValuationMethod method, string tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Calculating item valuation for {Item} on {Date}", itemCode, valuationDate);

            var snapshots = await _snapshotRepository.GetByItemCodeAsync(itemCode, tenantId, cancellationToken);
            
            var snapshot = snapshots
                .Where(s => s.SnapshotDate.Date == valuationDate.Date && s.Method == method)
                .OrderByDescending(s => s.SnapshotDate)
                .FirstOrDefault();

            return snapshot?.TotalValue ?? Money.Zero();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating item valuation for {Item}", itemCode);
            throw;
        }
    }

    public async Task<Guid> CreateStockValuationSnapshotAsync(DateTime snapshotDate, ValuationMethod method, string tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating stock valuation snapshot for {Date} using {Method}", snapshotDate, method);

            // For MVP, we'll create a mock snapshot
            // In a real implementation, this would integrate with the Inventory service to get actual stock levels
            var mockSnapshots = CreateMockStockSnapshots(snapshotDate, method, tenantId);
            
            foreach (var snapshot in mockSnapshots)
            {
                await _snapshotRepository.AddAsync(snapshot, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created {Count} stock valuation snapshots", mockSnapshots.Count);
            
            // Return the first snapshot ID as a reference
            return mockSnapshots.First().Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating stock valuation snapshot for {Date}", snapshotDate);
            throw;
        }
    }

    public async Task<Money> GetTotalStockValueForPLAsync(DateTime asOfDate, string tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting total stock value for P&L as of {Date}", asOfDate);

            var snapshots = await _snapshotRepository.GetByDateAsync(asOfDate, tenantId, cancellationToken);
            
            var totalValue = snapshots.Sum(s => s.TotalValue.Amount);
            
            return new Money(totalValue, "ZAR");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total stock value for P&L as of {Date}", asOfDate);
            throw;
        }
    }

    public async Task<StockValuationSummary> GetStockValuationSummaryAsync(DateTime asOfDate, string tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting stock valuation summary as of {Date}", asOfDate);

            var snapshots = await _snapshotRepository.GetByDateAsync(asOfDate, tenantId, cancellationToken);
            
            var totalValue = snapshots.Sum(s => s.TotalValue.Amount);
            var itemCount = snapshots.Select(s => s.ItemCode).Distinct().Count();
            var warehouseCount = snapshots.Select(s => s.WarehouseCode).Distinct().Count();
            
            var warehouseValues = snapshots
                .GroupBy(s => s.WarehouseCode)
                .ToDictionary(g => g.Key, g => new Money(g.Sum(s => s.TotalValue.Amount), "ZAR"));
            
            var categoryValues = snapshots
                .GroupBy(s => GetItemCategory(s.ItemCode))
                .ToDictionary(g => g.Key, g => new Money(g.Sum(s => s.TotalValue.Amount), "ZAR"));

            return new StockValuationSummary
            {
                AsOfDate = asOfDate,
                TotalValue = new Money(totalValue, "ZAR"),
                ItemCount = itemCount,
                WarehouseCount = warehouseCount,
                Method = ValuationMethod.WeightedAverage, // Default for MVP
                WarehouseValues = warehouseValues,
                CategoryValues = categoryValues
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stock valuation summary as of {Date}", asOfDate);
            throw;
        }
    }

    private List<StockValuationSnapshot> CreateMockStockSnapshots(DateTime snapshotDate, ValuationMethod method, string tenantId)
    {
        var snapshots = new List<StockValuationSnapshot>();
        
        // Mock inventory data for MVP
        var mockItems = new[]
        {
            new { Code = "ITEM001", Name = "Laptop", Quantity = 10m, UnitCost = 5000m, Warehouse = "WH001" },
            new { Code = "ITEM002", Name = "Mouse", Quantity = 50m, UnitCost = 200m, Warehouse = "WH001" },
            new { Code = "ITEM003", Name = "Keyboard", Quantity = 25m, UnitCost = 300m, Warehouse = "WH001" },
            new { Code = "ITEM004", Name = "Monitor", Quantity = 15m, UnitCost = 1500m, Warehouse = "WH002" },
            new { Code = "ITEM005", Name = "Headphones", Quantity = 30m, UnitCost = 150m, Warehouse = "WH002" }
        };

        // Get or create inventory account
        var inventoryAccount = GetOrCreateInventoryAccount(tenantId);

        foreach (var item in mockItems)
        {
            var snapshot = StockValuationSnapshot.Create(
                snapshotDate,
                item.Warehouse,
                item.Code,
                item.Name,
                item.Quantity,
                new Money(item.UnitCost, "ZAR"),
                method,
                inventoryAccount,
                tenantId,
                $"Mock snapshot for {item.Name}"
            );
            
            snapshots.Add(snapshot);
        }

        return snapshots;
    }

    private Guid GetOrCreateInventoryAccount(string tenantId)
    {
        // For MVP, return a mock account ID
        // In a real implementation, this would query the account repository
        return Guid.Parse("11111111-1111-1111-1111-111111111111");
    }

    private string GetItemCategory(string itemCode)
    {
        // Simple categorization for MVP
        return itemCode switch
        {
            "ITEM001" or "ITEM004" => "Computers",
            "ITEM002" or "ITEM003" => "Peripherals",
            "ITEM005" => "Audio",
            _ => "Other"
        };
    }
}
