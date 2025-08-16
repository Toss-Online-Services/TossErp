using Microsoft.EntityFrameworkCore;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Infrastructure.Data;

namespace TossErp.Stock.Infrastructure.Repositories;

public class StockEntryRepository : IStockEntryRepository
{
    private readonly ApplicationDbContext _context;

    public StockEntryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StockEntryAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.StockEntries
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<StockEntryAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StockEntries
            .Include(x => x.Details)
            .ToListAsync(cancellationToken);
    }

    public async Task<StockEntryAggregate> AddAsync(StockEntryAggregate stockEntry, CancellationToken cancellationToken = default)
    {
        _context.StockEntries.Add(stockEntry);
        await _context.SaveChangesAsync(cancellationToken);
        return stockEntry;
    }

    public async Task UpdateAsync(StockEntryAggregate stockEntry, CancellationToken cancellationToken = default)
    {
        _context.StockEntries.Update(stockEntry);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(StockEntryAggregate stockEntry, CancellationToken cancellationToken = default)
    {
        _context.StockEntries.Remove(stockEntry);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var stockEntry = await GetByIdAsync(id, cancellationToken);
        if (stockEntry != null)
        {
            await DeleteAsync(stockEntry, cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.StockEntries.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StockEntries.CountAsync(cancellationToken);
    }

    public void Add(StockEntryAggregate stockEntry)
    {
        _context.StockEntries.Add(stockEntry);
    }

    public void Update(StockEntryAggregate stockEntry)
    {
        _context.StockEntries.Update(stockEntry);
    }

    public void Delete(StockEntryAggregate stockEntry)
    {
        _context.StockEntries.Remove(stockEntry);
    }

    // Custom methods matching actual StockEntry properties
    public async Task<StockEntryAggregate?> GetByEntryNumberAsync(string entryNumber, CancellationToken cancellationToken = default)
    {
        return await _context.StockEntries
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.EntryNumber == entryNumber, cancellationToken);
    }

    public async Task<IEnumerable<StockEntryAggregate>> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default)
    {
        return await _context.StockEntries
            .Include(x => x.Details)
            .Where(x => x.Reference == reference)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockEntryAggregate>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.StockEntries
            .Include(x => x.Details)
            .Where(x => x.EntryDate >= fromDate && x.EntryDate <= toDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockEntryAggregate>> GetPostedEntriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StockEntries
            .Include(x => x.Details)
            .Where(x => x.IsPosted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockEntryAggregate>> GetUnpostedEntriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StockEntries
            .Include(x => x.Details)
            .Where(x => !x.IsPosted)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> GetPostedCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StockEntries
            .Where(x => x.IsPosted)
            .CountAsync(cancellationToken);
    }

    public async Task<long> GetUnpostedCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StockEntries
            .Where(x => !x.IsPosted)
            .CountAsync(cancellationToken);
    }

    // Stubbed methods for interface compatibility (not implemented due to missing properties)
    public Task<StockEntryAggregate?> GetByNameAsync(string name, CancellationToken cancellationToken = default) => Task.FromResult<StockEntryAggregate?>(null);
    public Task<IEnumerable<StockEntryAggregate>> GetByTypeAsync(StockEntryType stockEntryType, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetByPurposeAsync(StockEntryPurpose purpose, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetByStatusAsync(StockEntryStatus status, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetByCompanyAsync(string company, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetByWarehouseAsync(string warehouseCode, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetByFromWarehouseAsync(string warehouseCode, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetByToWarehouseAsync(string warehouseCode, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetByItemAsync(string itemCode, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetByWorkOrderAsync(string workOrder, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetByPurchaseOrderAsync(string purchaseOrder, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetByDeliveryNoteAsync(string deliveryNoteNo, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetBySalesInvoiceAsync(string salesInvoiceNo, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetByPickListAsync(string pickList, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetByPurchaseReceiptAsync(string purchaseReceiptNo, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetByAssetRepairAsync(string assetRepair, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetByProjectAsync(string project, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetBySupplierAsync(string supplier, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetOpeningEntriesAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetReturnEntriesAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetTransitEntriesAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetByBOMAsync(string bomNo, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) => Task.FromResult(false);
    public Task<long> GetCountByTypeAsync(StockEntryType stockEntryType, CancellationToken cancellationToken = default) => Task.FromResult(0L);
    public Task<long> GetCountByPurposeAsync(StockEntryPurpose purpose, CancellationToken cancellationToken = default) => Task.FromResult(0L);
    public Task<long> GetCountByStatusAsync(StockEntryStatus status, CancellationToken cancellationToken = default) => Task.FromResult(0L);
    public Task<long> GetCountByCompanyAsync(string company, CancellationToken cancellationToken = default) => Task.FromResult(0L);
    public Task<decimal> GetTotalValueByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default) => Task.FromResult(0m);
    public Task<decimal> GetTotalValueByPurposeAsync(StockEntryPurpose purpose, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default) => Task.FromResult(0m);

    // Missing interface methods
    public Task<IEnumerable<StockEntryAggregate>> GetByItemAsync(Guid itemId, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<IEnumerable<StockEntryAggregate>> GetByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<StockEntryAggregate>>(Array.Empty<StockEntryAggregate>());
    public Task<bool> ExistsByEntryNumberAsync(string entryNumber, CancellationToken cancellationToken = default) => Task.FromResult(false);
    public Task<string> GetNextEntryNumberAsync(string prefix, CancellationToken cancellationToken = default) => Task.FromResult($"{prefix}-{DateTime.Now:yyyyMMdd}-001");

    public async Task<IEnumerable<StockEntryAggregate>> GetPendingEntriesAsync(CancellationToken cancellationToken = default)
    {
        // Get stock entries that are not posted and ready for processing
        return await _context.StockEntries
            .Include(x => x.Details)
            .Where(x => !x.IsPosted && x.Status == StockEntryStatus.Pending)
            .ToListAsync(cancellationToken);
    }

    public IQueryable<StockEntryAggregate> GetQueryable()
    {
        return _context.StockEntries
            .Include(x => x.Details)
            .AsQueryable();
    }
} 
