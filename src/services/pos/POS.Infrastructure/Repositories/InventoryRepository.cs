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
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Inventory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Inventories
            .OrderBy(i => i.ProductId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Inventory>> GetAsync(Specification<Inventory> specification, CancellationToken cancellationToken = default)
    {
        var query = _context.Inventories.AsQueryable();
        if (specification != null)
        {
            var predicate = specification.ToExpression();
            query = query.Where(predicate);
        }
        return await query.OrderBy(i => i.ProductId).ToListAsync(cancellationToken);
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

    public async Task<int> CountAsync(Specification<Inventory> specification, CancellationToken cancellationToken = default)
    {
        var query = _context.Inventories.AsQueryable();
        if (specification != null)
        {
            var predicate = specification.ToExpression();
            query = query.Where(predicate);
        }
        return await query.CountAsync(cancellationToken);
    }

    // Overloads without CancellationToken
    public Task<Inventory> AddAsync(Inventory inventory) => AddAsync(inventory, default);
    public Task UpdateAsync(Inventory inventory) => UpdateAsync(inventory, default);
    public Task DeleteAsync(Inventory inventory) => DeleteAsync(inventory, default);

    public async Task<Inventory> GetAsync(int inventoryId)
    {
        var inventory = await _context.Inventories
            .Include(i => i.Product)
            .Include(i => i.Store)
            .FirstOrDefaultAsync(i => i.Id == inventoryId);

        return inventory;
    }

    public async Task<Inventory> GetByProductAndStoreAsync(int productId, int storeId)
    {
        var inventory = await _context.Inventories
            .Include(i => i.Product)
            .Include(i => i.Store)
            .FirstOrDefaultAsync(i => i.Product.Id == productId && i.Store.Id == storeId);

        return inventory;
    }

    public async Task<IEnumerable<Inventory>> GetByStoreAsync(int storeId)
    {
        return await _context.Inventories
            .Include(i => i.Product)
            .Where(i => i.Store.Id == storeId)
            .OrderBy(i => i.Product.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Inventory>> GetLowStockAsync(int storeId)
    {
        return await _context.Inventories
            .Include(i => i.Product)
            .Where(i => i.Store.Id == storeId && i.Quantity <= i.MinQuantity)
            .OrderBy(i => i.Quantity)
            .ToListAsync();
    }

    public async Task<IEnumerable<Inventory>> GetOverstockAsync(int storeId)
    {
        return await _context.Inventories
            .Include(i => i.Product)
            .Where(i => i.Store.Id == storeId && i.Quantity >= i.MaxQuantity)
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
