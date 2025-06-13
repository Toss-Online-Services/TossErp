using Microsoft.EntityFrameworkCore;
using eShop.POS.Domain.Repositories;
using eShop.POS.Infrastructure.Data;

namespace eShop.POS.Infrastructure.Repositories;

public class SyncLogRepository : ISyncLogRepository
{
    private readonly POSContext _context;

    public SyncLogRepository(POSContext context)
    {
        _context = context;
    }

    public async Task RecordSync(int saleId, string storeId, DateTime syncedAt, CancellationToken cancellationToken = default)
    {
        var syncLog = new SyncLog
        {
            SaleId = saleId,
            StoreId = storeId,
            SyncedAt = syncedAt,
            Success = true
        };

        await _context.SyncLogs.AddAsync(syncLog, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
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
