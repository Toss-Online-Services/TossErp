using TossErp.POS.Domain.AggregatesModel.StoreAggregate;
using TossErp.POS.Domain.Common;
using TossErp.POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TossErp.POS.Infrastructure.Repositories;

public class StoreRepository : IStoreRepository
{
    private readonly POSContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public StoreRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Store?> GetByIdAsync(string id)
    {
        return await _context.Stores.FindAsync(id);
    }

    public async Task<IEnumerable<Store>> GetAllAsync()
    {
        return await _context.Stores.ToListAsync();
    }

    public async Task AddAsync(Store store)
    {
        await _context.Stores.AddAsync(store);
    }

    public void Update(Store store)
    {
        _context.Entry(store).State = EntityState.Modified;
    }

    public void Delete(Store store)
    {
        _context.Stores.Remove(store);
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Stores.AnyAsync(s => s.Id == id);
    }

    public async Task<Store?> GetByCodeAsync(string code)
    {
        return await _context.Stores.FirstOrDefaultAsync(s => s.Code == code);
    }

    public async Task<Store?> GetByNameAsync(string name)
    {
        return await _context.Stores.FirstOrDefaultAsync(s => s.Name == name);
    }

    public async Task<Store?> GetByEmailAsync(string email)
    {
        return await _context.Stores.FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<IEnumerable<Store>> GetByRegionAsync(string region)
    {
        return await _context.Stores
            .Where(s => s.Region == region)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Store>> GetByOwnerIdAsync(string ownerId)
    {
        return await _context.Stores
            .Where(s => s.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Store>> GetByStatusAsync(bool isActive)
    {
        return await _context.Stores
            .Where(s => s.IsActive == isActive)
            .ToListAsync();
    }

    public async Task<Store?> GetAsync(string storeId)
    {
        return await _context.Stores
            .FirstOrDefaultAsync(s => s.Id == storeId);
    }

    public async Task<IEnumerable<string>> GetNotificationTokensAsync(string storeId)
    {
        var store = await GetAsync(storeId);
        return store?.NotificationTokens ?? Enumerable.Empty<string>();
    }
} 
