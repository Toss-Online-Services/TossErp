using System.Linq.Expressions;
using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using IStoreRepository = POS.Domain.Repositories.IStoreRepository;
using POS.Domain.Common;
using POS.Domain.SeedWork;

namespace TossErp.POS.Infrastructure.Repositories;

public class StoreRepository : IStoreRepository
{
    private readonly POSContext _context;

    public StoreRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Store?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Store>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Store>> FindAsync(Expression<Func<Store, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Store entity, CancellationToken cancellationToken = default)
    {
        await _context.Stores.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(Store entity, CancellationToken cancellationToken = default)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var store = await _context.Stores.FindAsync(new object[] { id }, cancellationToken);
        if (store != null)
        {
            _context.Stores.Remove(store);
        }
    }

    public async Task<IEnumerable<Store>> GetByRegionAsync(string region, CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .Where(s => s.Region == region)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Store>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .Where(s => s.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<Store?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .FirstOrDefaultAsync(s => s.Code == code, cancellationToken);
    }

    public async Task<Store?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .FirstOrDefaultAsync(s => s.Phone == phone, cancellationToken);
    }

    public async Task<Store?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
    }

    public async Task<Store?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .FirstOrDefaultAsync(s => s.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<Store>> GetByOwnerIdAsync(string ownerId)
    {
        return await _context.Stores
            .Where(s => s.OwnerId == ownerId)
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
