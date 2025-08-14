using Microsoft.EntityFrameworkCore;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Aggregates;
using TossErp.Stock.Infrastructure.Data;

namespace TossErp.Stock.Infrastructure.Repositories;

public class StockLevelRepository : IStockLevelRepository
{
    private readonly ApplicationDbContext _context;

    public StockLevelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StockLevel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.StockLevels
            .Include(sl => sl.Item)
            .Include(sl => sl.Warehouse)
            .Include(sl => sl.Bin)
            .FirstOrDefaultAsync(sl => sl.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<StockLevel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StockLevels
            .Include(sl => sl.Item)
            .Include(sl => sl.Warehouse)
            .Include(sl => sl.Bin)
            .ToListAsync(cancellationToken);
    }

    public async Task<StockLevel?> GetByItemAndWarehouseAsync(Guid itemId, Guid warehouseId, Guid? binId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.StockLevels
            .Include(sl => sl.Item)
            .Include(sl => sl.Warehouse)
            .Include(sl => sl.Bin)
            .Where(sl => sl.ItemId == itemId && sl.WarehouseId == warehouseId);

        if (binId.HasValue)
            query = query.Where(sl => sl.BinId == binId);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockLevel>> GetByItemAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        return await _context.StockLevels
            .Include(sl => sl.Item)
            .Include(sl => sl.Warehouse)
            .Include(sl => sl.Bin)
            .Where(sl => sl.ItemId == itemId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockLevel>> GetByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await _context.StockLevels
            .Include(sl => sl.Item)
            .Include(sl => sl.Warehouse)
            .Include(sl => sl.Bin)
            .Where(sl => sl.WarehouseId == warehouseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockLevel>> GetLowStockItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StockLevels
            .Include(sl => sl.Item)
            .Include(sl => sl.Warehouse)
            .Include(sl => sl.Bin)
            .Where(sl => sl.Item != null && sl.Item.ReOrderLevel.HasValue && sl.Quantity <= sl.Item.ReOrderLevel.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalQuantityAsync(Guid itemId, Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await _context.StockLevels
            .Where(sl => sl.ItemId == itemId && sl.WarehouseId == warehouseId)
            .SumAsync(sl => sl.Quantity, cancellationToken);
    }

    public async Task<bool> HasStockAsync(Guid itemId, Guid warehouseId, decimal quantity, CancellationToken cancellationToken = default)
    {
        var availableStock = await _context.StockLevels
            .Where(sl => sl.ItemId == itemId && sl.WarehouseId == warehouseId)
            .SumAsync(sl => sl.Quantity - sl.ReservedQuantity, cancellationToken);

        return availableStock >= quantity;
    }

    public async Task<StockLevel> AddAsync(StockLevel entity, CancellationToken cancellationToken = default)
    {
        await _context.StockLevels.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(StockLevel entity, CancellationToken cancellationToken = default)
    {
        _context.StockLevels.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(StockLevel entity, CancellationToken cancellationToken = default)
    {
        _context.StockLevels.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            _context.StockLevels.Remove(entity);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.StockLevels.AnyAsync(sl => sl.Id == id, cancellationToken);
    }

    public async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StockLevels.LongCountAsync(cancellationToken);
    }

    public void Add(StockLevel entity)
    {
        _context.StockLevels.Add(entity);
    }

    public void Update(StockLevel entity)
    {
        _context.StockLevels.Update(entity);
    }

    public void Delete(StockLevel entity)
    {
        _context.StockLevels.Remove(entity);
    }

    public IQueryable<StockLevel> GetQueryable()
    {
        return _context.StockLevels
            .Include(sl => sl.Item)
            .Include(sl => sl.Warehouse)
            .Include(sl => sl.Bin);
    }
}
