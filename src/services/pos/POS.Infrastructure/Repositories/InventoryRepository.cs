#nullable enable

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.InventoryAggregate;
using POS.Domain.SeedWork;
using POS.Infrastructure.EntityConfigurations;

namespace POS.Infrastructure.Repositories;

public class InventoryRepository : IRepository<Inventory>
{
    private readonly POSContext _context;

    public InventoryRepository(POSContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Inventory> AddAsync(Inventory inventory, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Inventories.AddAsync(inventory, cancellationToken);
        return entry.Entity;
    }

    public async Task<Inventory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Inventories
            .Include(i => i.Movements)
            .Include(i => i.Reservations)
            .Include(i => i.Adjustments)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Inventory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Inventories
            .Include(i => i.Movements)
            .Include(i => i.Reservations)
            .Include(i => i.Adjustments)
            .OrderBy(i => i.ProductId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Inventory>> GetAsync(Expression<Func<Inventory, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Inventories
            .Include(i => i.Movements)
            .Include(i => i.Reservations)
            .Include(i => i.Adjustments)
            .Where(predicate)
            .OrderBy(i => i.ProductId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Inventory inventory, CancellationToken cancellationToken = default)
    {
        _context.Entry(inventory).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Inventory inventory, CancellationToken cancellationToken = default)
    {
        _context.Inventories.Remove(inventory);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var inventory = await _context.Inventories.FindAsync(new object[] { id }, cancellationToken);
        if (inventory != null)
        {
            _context.Inventories.Remove(inventory);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Inventories
            .AnyAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Inventory, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Inventories.CountAsync(predicate, cancellationToken);
    }

    // Overloads without CancellationToken
    public Task<Inventory> AddAsync(Inventory inventory) => AddAsync(inventory, default);
    public Task UpdateAsync(Inventory inventory) => UpdateAsync(inventory, default);
    public Task DeleteAsync(Inventory inventory) => DeleteAsync(inventory, default);

    public async Task<Inventory?> GetAsync(Guid inventoryId)
    {
        var inventory = await _context.Inventories
            .Include(i => i.Movements)
            .Include(i => i.Reservations)
            .Include(i => i.Adjustments)
            .FirstOrDefaultAsync(i => i.Id == inventoryId);

        return inventory;
    }

    public async Task<Inventory?> GetByProductAndStoreAsync(Guid productId, Guid storeId)
    {
        var inventory = await _context.Inventories
            .Include(i => i.Movements)
            .Include(i => i.Reservations)
            .Include(i => i.Adjustments)
            .FirstOrDefaultAsync(i => i.ProductId == productId && i.StoreId == storeId);

        return inventory;
    }

    public async Task<IEnumerable<Inventory>> GetByStoreAsync(Guid storeId)
    {
        return await _context.Inventories
            .Include(i => i.Movements)
            .Include(i => i.Reservations)
            .Include(i => i.Adjustments)
            .Where(i => i.StoreId == storeId)
            .OrderBy(i => i.ProductId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Inventory>> GetLowStockAsync(Guid storeId)
    {
        return await _context.Inventories
            .Include(i => i.Movements)
            .Include(i => i.Reservations)
            .Include(i => i.Adjustments)
            .Where(i => i.StoreId == storeId && i.Quantity <= i.MinimumStock)
            .OrderBy(i => i.Quantity)
            .ToListAsync();
    }

    public async Task<IEnumerable<Inventory>> GetOverstockAsync(Guid storeId)
    {
        return await _context.Inventories
            .Include(i => i.Movements)
            .Include(i => i.Reservations)
            .Include(i => i.Adjustments)
            .Where(i => i.StoreId == storeId && i.Quantity >= i.MaximumStock)
            .OrderByDescending(i => i.Quantity)
            .ToListAsync();
    }

    public void Update(Inventory inventory)
    {
        _context.Entry(inventory).State = EntityState.Modified;
    }

    public void Delete(Inventory inventory)
    {
        _context.Inventories.Remove(inventory);
    }
} 
