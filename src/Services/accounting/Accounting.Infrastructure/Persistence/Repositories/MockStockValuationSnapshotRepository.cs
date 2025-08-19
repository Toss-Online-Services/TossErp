using TossErp.Accounting.Application.Common.Interfaces;
using TossErp.Accounting.Domain.Common;
using TossErp.Accounting.Domain.Entities;
using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Infrastructure.Persistence.Repositories;

/// <summary>
/// Mock implementation of IStockValuationSnapshotRepository for testing
/// </summary>
public class MockStockValuationSnapshotRepository : IStockValuationSnapshotRepository
{
    private readonly List<StockValuationSnapshot> _snapshots = new();

    public MockStockValuationSnapshotRepository()
    {
        // Initialize with some mock data
        InitializeMockData();
    }

    public Task<StockValuationSnapshot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var snapshot = _snapshots.FirstOrDefault(s => s.Id == id);
        return Task.FromResult(snapshot);
    }

    public Task<IEnumerable<StockValuationSnapshot>> GetByDateAsync(DateTime snapshotDate, string tenantId, CancellationToken cancellationToken = default)
    {
        var snapshots = _snapshots.Where(s => s.SnapshotDate.Date == snapshotDate.Date && s.TenantId == tenantId);
        return Task.FromResult(snapshots);
    }

    public Task<IEnumerable<StockValuationSnapshot>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, string tenantId, CancellationToken cancellationToken = default)
    {
        var snapshots = _snapshots.Where(s => s.SnapshotDate.Date >= fromDate.Date && 
                                             s.SnapshotDate.Date <= toDate.Date && 
                                             s.TenantId == tenantId);
        return Task.FromResult(snapshots);
    }

    public Task<IEnumerable<StockValuationSnapshot>> GetByWarehouseAsync(string warehouseCode, string tenantId, CancellationToken cancellationToken = default)
    {
        var snapshots = _snapshots.Where(s => s.WarehouseCode == warehouseCode && s.TenantId == tenantId);
        return Task.FromResult(snapshots);
    }

    public Task<IEnumerable<StockValuationSnapshot>> GetByItemCodeAsync(string itemCode, string tenantId, CancellationToken cancellationToken = default)
    {
        var snapshots = _snapshots.Where(s => s.ItemCode == itemCode && s.TenantId == tenantId);
        return Task.FromResult(snapshots);
    }

    public Task<IEnumerable<StockValuationSnapshot>> GetByValuationMethodAsync(ValuationMethod method, string tenantId, CancellationToken cancellationToken = default)
    {
        var snapshots = _snapshots.Where(s => s.Method == method && s.TenantId == tenantId);
        return Task.FromResult(snapshots);
    }

    public Task<IEnumerable<StockValuationSnapshot>> GetLatestSnapshotsAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        var latestSnapshots = _snapshots
            .Where(s => s.TenantId == tenantId)
            .GroupBy(s => new { s.ItemCode, s.WarehouseCode })
            .Select(g => g.OrderByDescending(s => s.SnapshotDate).First())
            .ToList();
        
        return Task.FromResult<IEnumerable<StockValuationSnapshot>>(latestSnapshots);
    }

    public Task<StockValuationSnapshot> AddAsync(StockValuationSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        _snapshots.Add(snapshot);
        return Task.FromResult(snapshot);
    }

    public Task<StockValuationSnapshot> UpdateAsync(StockValuationSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        var existingIndex = _snapshots.FindIndex(s => s.Id == snapshot.Id);
        if (existingIndex >= 0)
        {
            _snapshots[existingIndex] = snapshot;
        }
        return Task.FromResult(snapshot);
    }

    public Task DeleteAsync(StockValuationSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        _snapshots.RemoveAll(s => s.Id == snapshot.Id);
        return Task.CompletedTask;
    }

    private void InitializeMockData()
    {
        var tenantId = "tenant-001";
        var inventoryAccountId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var today = DateTime.Today;

        // Create mock snapshots for different dates
        var mockSnapshots = new[]
        {
            new { Date = today.AddDays(-30), ItemCode = "ITEM001", ItemName = "Laptop", Quantity = 8m, UnitCost = 5000m, Warehouse = "WH001" },
            new { Date = today.AddDays(-30), ItemCode = "ITEM002", ItemName = "Mouse", Quantity = 45m, UnitCost = 200m, Warehouse = "WH001" },
            new { Date = today.AddDays(-30), ItemCode = "ITEM003", ItemName = "Keyboard", Quantity = 22m, UnitCost = 300m, Warehouse = "WH001" },
            new { Date = today.AddDays(-30), ItemCode = "ITEM004", ItemName = "Monitor", Quantity = 12m, UnitCost = 1500m, Warehouse = "WH002" },
            new { Date = today.AddDays(-30), ItemCode = "ITEM005", ItemName = "Headphones", Quantity = 28m, UnitCost = 150m, Warehouse = "WH002" },
            
            new { Date = today.AddDays(-15), ItemCode = "ITEM001", ItemName = "Laptop", Quantity = 9m, UnitCost = 5000m, Warehouse = "WH001" },
            new { Date = today.AddDays(-15), ItemCode = "ITEM002", ItemName = "Mouse", Quantity = 48m, UnitCost = 200m, Warehouse = "WH001" },
            new { Date = today.AddDays(-15), ItemCode = "ITEM003", ItemName = "Keyboard", Quantity = 24m, UnitCost = 300m, Warehouse = "WH001" },
            new { Date = today.AddDays(-15), ItemCode = "ITEM004", ItemName = "Monitor", Quantity = 13m, UnitCost = 1500m, Warehouse = "WH002" },
            new { Date = today.AddDays(-15), ItemCode = "ITEM005", ItemName = "Headphones", Quantity = 29m, UnitCost = 150m, Warehouse = "WH002" },
            
            new { Date = today, ItemCode = "ITEM001", ItemName = "Laptop", Quantity = 10m, UnitCost = 5000m, Warehouse = "WH001" },
            new { Date = today, ItemCode = "ITEM002", ItemName = "Mouse", Quantity = 50m, UnitCost = 200m, Warehouse = "WH001" },
            new { Date = today, ItemCode = "ITEM003", ItemName = "Keyboard", Quantity = 25m, UnitCost = 300m, Warehouse = "WH001" },
            new { Date = today, ItemCode = "ITEM004", ItemName = "Monitor", Quantity = 15m, UnitCost = 1500m, Warehouse = "WH002" },
            new { Date = today, ItemCode = "ITEM005", ItemName = "Headphones", Quantity = 30m, UnitCost = 150m, Warehouse = "WH002" }
        };

        foreach (var item in mockSnapshots)
        {
            var snapshot = StockValuationSnapshot.Create(
                item.Date,
                item.Warehouse,
                item.ItemCode,
                item.ItemName,
                item.Quantity,
                new Money(item.UnitCost, "ZAR"),
                ValuationMethod.WeightedAverage,
                inventoryAccountId,
                tenantId,
                $"Mock snapshot for {item.ItemName}"
            );
            
            _snapshots.Add(snapshot);
        }
    }
}

