using Microsoft.EntityFrameworkCore;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Infrastructure.Data;

namespace TossErp.Stock.Infrastructure.Repositories;

public class StockMovementRepository : IStockMovementRepository
{
    private readonly ApplicationDbContext _context;

    public StockMovementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StockMovement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements
            .Include(sm => sm.Item)
            .Include(sm => sm.Warehouse)
            .Include(sm => sm.Bin)
            .Include(sm => sm.Batch)
            .FirstOrDefaultAsync(sm => sm.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<StockMovement>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements
            .Include(sm => sm.Item)
            .Include(sm => sm.Warehouse)
            .Include(sm => sm.Bin)
            .Include(sm => sm.Batch)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockMovement>> GetByItemAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements
            .Include(sm => sm.Item)
            .Include(sm => sm.Warehouse)
            .Include(sm => sm.Bin)
            .Include(sm => sm.Batch)
            .Where(sm => sm.ItemId == itemId)
            .OrderByDescending(sm => sm.MovementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockMovement>> GetByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements
            .Include(sm => sm.Item)
            .Include(sm => sm.Warehouse)
            .Include(sm => sm.Bin)
            .Include(sm => sm.Batch)
            .Where(sm => sm.WarehouseId == warehouseId)
            .OrderByDescending(sm => sm.MovementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockMovement>> GetByItemAndWarehouseAsync(Guid itemId, Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements
            .Include(sm => sm.Item)
            .Include(sm => sm.Warehouse)
            .Include(sm => sm.Bin)
            .Include(sm => sm.Batch)
            .Where(sm => sm.ItemId == itemId && sm.WarehouseId == warehouseId)
            .OrderByDescending(sm => sm.MovementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockMovement>> GetByMovementTypeAsync(MovementType movementType, CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements
            .Include(sm => sm.Item)
            .Include(sm => sm.Warehouse)
            .Include(sm => sm.Bin)
            .Include(sm => sm.Batch)
            .Where(sm => sm.MovementType == movementType)
            .OrderByDescending(sm => sm.MovementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockMovement>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements
            .Include(sm => sm.Item)
            .Include(sm => sm.Warehouse)
            .Include(sm => sm.Bin)
            .Include(sm => sm.Batch)
            .Where(sm => sm.MovementDate >= fromDate && sm.MovementDate <= toDate)
            .OrderByDescending(sm => sm.MovementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockMovement>> GetByReferenceAsync(string referenceNumber, string referenceType, CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements
            .Include(sm => sm.Item)
            .Include(sm => sm.Warehouse)
            .Include(sm => sm.Bin)
            .Include(sm => sm.Batch)
            .Where(sm => sm.ReferenceNumber == referenceNumber && sm.ReferenceType == referenceType)
            .OrderByDescending(sm => sm.MovementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockMovement>> GetByBatchAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements
            .Include(sm => sm.Item)
            .Include(sm => sm.Warehouse)
            .Include(sm => sm.Bin)
            .Include(sm => sm.Batch)
            .Where(sm => sm.BatchId == batchId)
            .OrderByDescending(sm => sm.MovementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<StockMovement> AddAsync(StockMovement entity, CancellationToken cancellationToken = default)
    {
        await _context.StockMovements.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(StockMovement entity, CancellationToken cancellationToken = default)
    {
        _context.StockMovements.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(StockMovement entity, CancellationToken cancellationToken = default)
    {
        _context.StockMovements.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            _context.StockMovements.Remove(entity);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements.AnyAsync(sm => sm.Id == id, cancellationToken);
    }

    public async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements.LongCountAsync(cancellationToken);
    }

    public void Add(StockMovement entity)
    {
        _context.StockMovements.Add(entity);
    }

    public void Update(StockMovement entity)
    {
        _context.StockMovements.Update(entity);
    }

    public void Delete(StockMovement entity)
    {
        _context.StockMovements.Remove(entity);
    }
}
