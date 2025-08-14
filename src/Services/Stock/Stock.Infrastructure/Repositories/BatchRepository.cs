using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TossErp.Stock.Infrastructure.Repositories;

public class BatchRepository : IBatchRepository
{
    private readonly ApplicationDbContext _context;
    public BatchRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public Task<Batch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Batch?>(null);
    public Task<IEnumerable<Batch>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Batch>>(new List<Batch>());
    public Task<Batch> AddAsync(Batch entity, CancellationToken cancellationToken = default) => Task.FromResult(entity);
    public Task UpdateAsync(Batch entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task DeleteAsync(Batch entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(false);
    public Task<long> GetCountAsync(CancellationToken cancellationToken = default) => Task.FromResult(0L);
    public void Add(Batch batch) { }
    public void Update(Batch batch) { }
    public void Delete(Batch batch) { }
    public Task<Batch?> GetByNameAsync(string name, CancellationToken cancellationToken = default) => Task.FromResult<Batch?>(null);
    public Task<IEnumerable<Batch>> GetByItemAsync(string itemCode, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Batch>>(Array.Empty<Batch>());
    public Task<IEnumerable<Batch>> GetBySupplierAsync(string supplier, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Batch>>(Array.Empty<Batch>());
    public Task<IEnumerable<Batch>> GetByReferenceDocumentAsync(string referenceDocumentType, string referenceDocumentNo, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Batch>>(Array.Empty<Batch>());
    public Task<IEnumerable<Batch>> GetExpiringBatchesAsync(int daysThreshold, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Batch>>(Array.Empty<Batch>());
    public Task<IEnumerable<Batch>> GetExpiredBatchesAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Batch>>(Array.Empty<Batch>());
    public Task<IEnumerable<Batch>> GetByExpiryDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Batch>>(Array.Empty<Batch>());
    public Task<IEnumerable<Batch>> GetByManufacturingDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Batch>>(Array.Empty<Batch>());
    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) => Task.FromResult(false);
    public Task<long> GetCountByItemAsync(string itemCode, CancellationToken cancellationToken = default) => Task.FromResult(0L);
    public Task<long> GetCountBySupplierAsync(string supplier, CancellationToken cancellationToken = default) => Task.FromResult(0L);
    public Task<long> GetExpiringCountAsync(int daysThreshold, CancellationToken cancellationToken = default) => Task.FromResult(0L);
    public Task<long> GetExpiredCountAsync(CancellationToken cancellationToken = default) => Task.FromResult(0L);
    public IQueryable<Batch> GetQueryable() => _context.Batches.AsQueryable();
} 
