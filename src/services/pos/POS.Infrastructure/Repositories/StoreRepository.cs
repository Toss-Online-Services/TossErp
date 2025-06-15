using System.Linq.Expressions;
using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;

namespace TossErp.POS.Infrastructure.Repositories;

public class StoreRepository : IStoreRepository
{
    private readonly POSContext _context;

    public StoreRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Store?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Stores.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Store>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Stores.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Store>> FindAsync(Expression<Func<Store, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Stores.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Store> AddAsync(Store store, CancellationToken cancellationToken = default)
    {
        await _context.Stores.AddAsync(store, cancellationToken);
        return store;
    }

    public void Update(Store store)
    {
        _context.Entry(store).State = EntityState.Modified;
    }

    public void Delete(Store store)
    {
        _context.Stores.Remove(store);
    }

    public async Task<Store> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Stores.FirstOrDefaultAsync(s => s.Code == code, cancellationToken);
    }

    public async Task<Store> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Stores.FirstOrDefaultAsync(s => s.Email == email, cancellationToken);
    }

    public async Task<Store> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        return await _context.Stores.FirstOrDefaultAsync(s => s.Phone == phone, cancellationToken);
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
