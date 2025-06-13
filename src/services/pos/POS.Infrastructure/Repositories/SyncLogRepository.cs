using TossErp.POS.Domain.AggregatesModel.SyncLogAggregate;
using TossErp.POS.Domain.Common;
using TossErp.POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TossErp.POS.Infrastructure.Repositories;

public class SyncLogRepository : ISyncLogRepository
{
    private readonly POSContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public SyncLogRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<SyncLog?> GetByIdAsync(string id)
    {
        return await _context.SyncLogs.FindAsync(id);
    }

    public async Task<IEnumerable<SyncLog>> GetAllAsync()
    {
        return await _context.SyncLogs.ToListAsync();
    }

    public async Task AddAsync(SyncLog syncLog)
    {
        await _context.SyncLogs.AddAsync(syncLog);
    }

    public void Update(SyncLog syncLog)
    {
        _context.Entry(syncLog).State = EntityState.Modified;
    }

    public void Delete(SyncLog syncLog)
    {
        _context.SyncLogs.Remove(syncLog);
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.SyncLogs.AnyAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<SyncLog>> GetByStoreIdAsync(string storeId)
    {
        return await _context.SyncLogs
            .Where(s => s.StoreId == storeId)
            .OrderByDescending(s => s.SyncDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<SyncLog>> GetByStatusAsync(SyncStatus status)
    {
        return await _context.SyncLogs
            .Where(s => s.Status == status)
            .OrderByDescending(s => s.SyncDate)
            .ToListAsync();
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
} 
