using Microsoft.EntityFrameworkCore;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Infrastructure.Data;

namespace TossErp.Stock.Infrastructure.Repositories;

public class SerialNoRepository : ISerialNoRepository
{
    private readonly ApplicationDbContext _context;

    public SerialNoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SerialNo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SerialNos
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<SerialNo?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        return await _context.SerialNos
            .FirstOrDefaultAsync(x => x.SerialNumber == serialNumber, cancellationToken);
    }

    public async Task<IEnumerable<SerialNo>> GetByItemAsync(string itemCode, CancellationToken cancellationToken = default)
    {
        // Note: This would need to join with Item table to filter by item code
        // For now, returning empty list as the relationship is not clear
        return await Task.FromResult<IEnumerable<SerialNo>>(new List<SerialNo>());
    }

    public async Task<IEnumerable<SerialNo>> GetByWarehouseAsync(string warehouse, CancellationToken cancellationToken = default)
    {
        // Note: SerialNo doesn't have direct warehouse relationship
        // This would need to be implemented based on business logic
        return await Task.FromResult<IEnumerable<SerialNo>>(new List<SerialNo>());
    }

    public async Task<IEnumerable<SerialNo>> GetByLocationAsync(string location, CancellationToken cancellationToken = default)
    {
        return await _context.SerialNos
            .Where(x => x.Location == location)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SerialNo>> GetByBatchNoAsync(string batchNo, CancellationToken cancellationToken = default)
    {
        return await _context.SerialNos
            .Where(x => x.BatchNo == batchNo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SerialNo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SerialNos.ToListAsync(cancellationToken);
    }

    public async Task<SerialNo> AddAsync(SerialNo serialNo, CancellationToken cancellationToken = default)
    {
        await _context.SerialNos.AddAsync(serialNo, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return serialNo;
    }

    public async Task UpdateAsync(SerialNo serialNo, CancellationToken cancellationToken = default)
    {
        _context.SerialNos.Update(serialNo);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(SerialNo serialNo, CancellationToken cancellationToken = default)
    {
        _context.SerialNos.Remove(serialNo);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var serialNo = await _context.SerialNos.FindAsync(id, cancellationToken);
        if (serialNo != null)
        {
            _context.SerialNos.Remove(serialNo);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SerialNos.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        return await _context.SerialNos.AnyAsync(x => x.SerialNumber == serialNumber, cancellationToken);
    }

    public async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SerialNos.LongCountAsync(cancellationToken);
    }

    public async Task<long> GetCountByItemAsync(string itemCode, CancellationToken cancellationToken = default)
    {
        // Note: This would need to join with Item table to filter by item code
        return await Task.FromResult(0L);
    }

    public async Task<long> GetCountByWarehouseAsync(string warehouse, CancellationToken cancellationToken = default)
    {
        // Note: SerialNo doesn't have direct warehouse relationship
        return await Task.FromResult(0L);
    }

    public async Task<IEnumerable<SerialNo>> GetExpiredSerialNosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SerialNos
            .Where(x => x.ExpiryDate.HasValue && x.ExpiryDate.Value <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SerialNo>> GetExpiringSerialNosAsync(int daysThreshold, CancellationToken cancellationToken = default)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);
        return await _context.SerialNos
            .Where(x => x.ExpiryDate.HasValue && x.ExpiryDate.Value <= thresholdDate)
            .ToListAsync(cancellationToken);
    }

    public void Add(SerialNo serialNo)
    {
        _context.SerialNos.Add(serialNo);
    }

    public void Update(SerialNo serialNo)
    {
        _context.SerialNos.Update(serialNo);
    }

    public void Delete(SerialNo serialNo)
    {
        _context.SerialNos.Remove(serialNo);
    }

    // Additional ISerialNoRepository methods
    public async Task<SerialNo?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.SerialNos
            .FirstOrDefaultAsync(x => x.SerialNumber == name, cancellationToken);
    }

    public Task<IEnumerable<SerialNo>> GetBySupplierAsync(string supplier, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<SerialNo>>(new List<SerialNo>());
    }

    public Task<IEnumerable<SerialNo>> GetByCustomerAsync(string customer, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<SerialNo>>(new List<SerialNo>());
    }

    public Task<IEnumerable<SerialNo>> GetByPurchaseDocumentAsync(string purchaseDocumentType, string purchaseDocumentNo, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<SerialNo>>(new List<SerialNo>());
    }

    public Task<IEnumerable<SerialNo>> GetBySalesDocumentAsync(string salesDocumentType, string salesDocumentNo, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<SerialNo>>(new List<SerialNo>());
    }

    public Task<IEnumerable<SerialNo>> GetByProjectAsync(string project, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<SerialNo>>(new List<SerialNo>());
    }

    public Task<IEnumerable<SerialNo>> GetByAssetAsync(string asset, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<SerialNo>>(new List<SerialNo>());
    }

    public Task<IEnumerable<SerialNo>> GetByMaintenanceStatusAsync(string maintenanceStatus, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<SerialNo>>(new List<SerialNo>());
    }

    public Task<IEnumerable<SerialNo>> GetByWarrantyExpiryRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<SerialNo>>(new List<SerialNo>());
    }

    public Task<IEnumerable<SerialNo>> GetByAMCExpiryRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<SerialNo>>(new List<SerialNo>());
    }

    public Task<IEnumerable<SerialNo>> GetExpiringWarrantyAsync(int daysThreshold, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<SerialNo>>(new List<SerialNo>());
    }

    public Task<IEnumerable<SerialNo>> GetExpiringAMCAsync(int daysThreshold, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<SerialNo>>(new List<SerialNo>());
    }

    public Task<IEnumerable<SerialNo>> GetExpiredWarrantyAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<SerialNo>>(new List<SerialNo>());
    }

    public Task<IEnumerable<SerialNo>> GetExpiredAMCAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<SerialNo>>(new List<SerialNo>());
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.SerialNos.AnyAsync(x => x.SerialNumber == name, cancellationToken);
    }

    public Task<long> GetCountBySupplierAsync(string supplier, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0L);
    }

    public Task<long> GetCountByCustomerAsync(string customer, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0L);
    }

    public Task<long> GetExpiringWarrantyCountAsync(int daysThreshold, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0L);
    }

    public Task<long> GetExpiringAMCCountAsync(int daysThreshold, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0L);
    }

    public Task<long> GetExpiredWarrantyCountAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0L);
    }

    public Task<long> GetExpiredAMCCountAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0L);
    }

    public IQueryable<SerialNo> GetQueryable()
    {
        return _context.SerialNos.AsQueryable();
    }
} 
