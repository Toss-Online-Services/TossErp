using Microsoft.EntityFrameworkCore;
using TossErp.Procurement.Domain.Common;
using TossErp.Procurement.Domain.Entities;
using TossErp.Procurement.Domain.Enums;
using TossErp.Procurement.Infrastructure.Persistence;

namespace TossErp.Procurement.Infrastructure.Persistence.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly ProcurementDbContext _context;

    public SupplierRepository(ProcurementDbContext context)
    {
        _context = context;
    }

    public async Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Supplier>> GetByStatusAsync(SupplierStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .Where(s => s.Status == status)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Supplier?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<Supplier>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .Where(s => s.Name.Contains(name) || s.Code.Contains(name))
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Supplier>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .Where(s => s.Status == SupplierStatus.Active)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Supplier>> GetPendingApprovalAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .Where(s => s.Status == SupplierStatus.PendingApproval)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        if (excludeId.HasValue)
        {
            return await _context.Suppliers
                .AnyAsync(s => s.Code == code && s.Id != excludeId.Value, cancellationToken);
        }
        
        return await _context.Suppliers
            .AnyAsync(s => s.Code == code, cancellationToken);
    }

    public async Task<Supplier> AddAsync(Supplier entity, CancellationToken cancellationToken = default)
    {
        await _context.Suppliers.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task<Supplier> UpdateAsync(Supplier entity, CancellationToken cancellationToken = default)
    {
        _context.Suppliers.Update(entity);
        return Task.FromResult(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            _context.Suppliers.Remove(entity);
            await Task.CompletedTask; // Add await to satisfy async requirement
        }
    }
}
