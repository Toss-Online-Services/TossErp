#nullable enable

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.SeedWork;
using POS.Infrastructure.EntityConfigurations;

namespace POS.Infrastructure.Repositories;

public class StoreRepository : IRepository<Store>
{
    private readonly POSContext _context;

    public StoreRepository(POSContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Store> AddAsync(Store store, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Stores.AddAsync(store, cancellationToken);
        return entry.Entity;
    }

    public async Task<Store?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .Include(s => s.StoreHours)
            .Include(s => s.Devices)
            .Include(s => s.Printers)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Store>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .Include(s => s.StoreHours)
            .Include(s => s.Devices)
            .Include(s => s.Printers)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Store>> GetAsync(Expression<Func<Store, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .Include(s => s.StoreHours)
            .Include(s => s.Devices)
            .Include(s => s.Printers)
            .Where(predicate)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Store store, CancellationToken cancellationToken = default)
    {
        _context.Entry(store).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Store store, CancellationToken cancellationToken = default)
    {
        _context.Stores.Remove(store);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var store = await _context.Stores.FindAsync(new object[] { id }, cancellationToken);
        if (store != null)
        {
            _context.Stores.Remove(store);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Store, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Stores.CountAsync(predicate, cancellationToken);
    }

    // Overloads without CancellationToken
    public Task<Store> AddAsync(Store store) => AddAsync(store, default);
    public Task UpdateAsync(Store store) => UpdateAsync(store, default);
    public Task DeleteAsync(Store store) => DeleteAsync(store, default);

    public async Task<Store?> GetByCodeAsync(string code)
    {
        return await _context.Stores
            .Include(s => s.StoreHours)
            .Include(s => s.Devices)
            .Include(s => s.Printers)
            .FirstOrDefaultAsync(s => s.Code == code);
    }

    public async Task<Store?> GetByEmailAsync(string email)
    {
        return await _context.Stores
            .Include(s => s.StoreHours)
            .Include(s => s.Devices)
            .Include(s => s.Printers)
            .FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<IEnumerable<Store>> GetActiveStoresAsync()
    {
        return await _context.Stores
            .Include(s => s.StoreHours)
            .Include(s => s.Devices)
            .Include(s => s.Printers)
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Store>> GetByTimeZoneAsync(string timeZone)
    {
        return await _context.Stores
            .Include(s => s.StoreHours)
            .Include(s => s.Devices)
            .Include(s => s.Printers)
            .Where(s => s.TimeZone == timeZone)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public void Update(Store store)
    {
        _context.Entry(store).State = EntityState.Modified;
    }

    public void Delete(Store store)
    {
        _context.Stores.Remove(store);
    }
} 
