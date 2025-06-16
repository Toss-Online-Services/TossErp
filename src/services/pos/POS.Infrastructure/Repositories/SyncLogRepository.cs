using System.Linq.Expressions;
using POS.Domain.AggregatesModel.SyncAggregate;
using POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ISyncLogRepository = POS.Domain.Repositories.ISyncLogRepository;

namespace TossErp.POS.Infrastructure.Repositories;

public class SyncLogRepository : ISyncLogRepository
{
    private readonly POSContext _context;

    public SyncLogRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<SyncLog?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.SyncLogs
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<SyncLog?> GetByEntityIdAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default)
    {
        return await _context.SyncLogs
            .FirstOrDefaultAsync(s => s.EntityType == entityType && s.EntityId == entityId, cancellationToken);
    }

    public async Task<IEnumerable<SyncLog>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SyncLogs
            .ToListAsync(cancellationToken);
    }

    public async Task<SyncLog?> FindAsync(Expression<Func<SyncLog, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.SyncLogs
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task AddAsync(SyncLog entity, CancellationToken cancellationToken = default)
    {
        await _context.SyncLogs.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(SyncLog entity, CancellationToken cancellationToken = default)
    {
        _context.SyncLogs.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var syncLog = await _context.SyncLogs.FindAsync(new object[] { id }, cancellationToken);
        if (syncLog != null)
        {
            _context.SyncLogs.Remove(syncLog);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<SyncLog>> GetByStoreIdAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _context.SyncLogs
            .Where(s => s.StoreId == storeId)
            .OrderByDescending(s => s.SyncDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SyncLog>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await _context.SyncLogs
            .Where(s => s.Status == status)
            .OrderByDescending(s => s.SyncDate)
            .ToListAsync(cancellationToken);
    }

    public async Task RecordSync(string saleId, string storeId, DateTime syncedAt, CancellationToken cancellationToken = default)
    {
        var syncLog = new SyncLog
        {
            SaleId = saleId,
            StoreId = storeId,
            SyncedAt = syncedAt,
            Success = true
        };

        await _context.SyncLogs.AddAsync(syncLog, cancellationToken);
    }

    public async Task<IEnumerable<SyncLog>> GetPendingSyncsAsync(string storeId, CancellationToken cancellationToken = default)
    {
        return await _context.SyncLogs
            .Where(s => s.StoreId == storeId && !s.Success)
            .OrderBy(s => s.SyncedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SyncLog>> GetFailedSyncsAsync(string storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.SyncLogs
            .Where(s => s.StoreId == storeId && 
                       !s.Success && 
                       s.SyncedAt >= startDate && 
                       s.SyncedAt <= endDate)
            .OrderByDescending(s => s.SyncedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<DateTime?> GetLastSyncTimeAsync(string storeId, CancellationToken cancellationToken = default)
    {
        var lastSync = await _context.SyncLogs
            .Where(s => s.StoreId == storeId && s.Success)
            .OrderByDescending(s => s.SyncedAt)
            .FirstOrDefaultAsync(cancellationToken);

        return lastSync?.SyncedAt;
    }

    public async Task<int> GetPendingSyncCountAsync(string storeId, CancellationToken cancellationToken = default)
    {
        return await _context.SyncLogs
            .CountAsync(s => s.StoreId == storeId && !s.Success, cancellationToken);
    }

    public async Task<SyncLog?> GetLastSyncByStoreIdAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _context.SyncLogs
            .Where(s => s.StoreId == storeId)
            .OrderByDescending(s => s.SyncDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<SyncLog>> GetByEntityTypeAsync(string entityType, CancellationToken cancellationToken = default)
    {
        return await _context.SyncLogs.Where(l => l.EntityType == entityType).ToListAsync(cancellationToken);
    }
} 
