using Microsoft.EntityFrameworkCore;
using TossErp.Procurement.Domain.Common;
using TossErp.Procurement.Domain.Entities;
using TossErp.Procurement.Domain.Enums;
using TossErp.Procurement.Infrastructure.Persistence;

namespace TossErp.Procurement.Infrastructure.Persistence.Repositories;

public class PurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly ProcurementDbContext _context;

    public PurchaseOrderRepository(ProcurementDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders
            .Include(po => po.Items)
            .FirstOrDefaultAsync(po => po.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders
            .Include(po => po.Items)
            .OrderByDescending(po => po.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetByStatusAsync(PurchaseOrderStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders
            .Include(po => po.Items)
            .Where(po => po.Status == status)
            .OrderByDescending(po => po.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders
            .Include(po => po.Items)
            .Where(po => po.SupplierId == supplierId)
            .OrderByDescending(po => po.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetPendingApprovalAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders
            .Include(po => po.Items)
            .Where(po => po.Status == PurchaseOrderStatus.Draft || po.Status == PurchaseOrderStatus.Submitted)
            .OrderByDescending(po => po.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetOverdueAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        return await _context.PurchaseOrders
            .Include(po => po.Items)
            .Where(po => po.Status == PurchaseOrderStatus.Sent && 
                        po.Items.Any(item => item.ExpectedDeliveryDate < today && item.ReceivedQuantity < item.Quantity))
            .OrderByDescending(po => po.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders
            .Include(po => po.Items)
            .Where(po => po.OrderDate >= startDate && po.OrderDate <= endDate)
            .OrderByDescending(po => po.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<PurchaseOrder?> GetByPurchaseOrderNumberAsync(string purchaseOrderNumber, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders
            .Include(po => po.Items)
            .FirstOrDefaultAsync(po => po.PurchaseOrderNumber.Value == purchaseOrderNumber, cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetPendingReceiptAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders
            .Include(po => po.Items)
            .Where(po => po.Status == PurchaseOrderStatus.Approved || po.Status == PurchaseOrderStatus.Sent)
            .OrderByDescending(po => po.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<PurchaseOrder> AddAsync(PurchaseOrder entity, CancellationToken cancellationToken = default)
    {
        await _context.PurchaseOrders.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task<PurchaseOrder> UpdateAsync(PurchaseOrder entity, CancellationToken cancellationToken = default)
    {
        _context.PurchaseOrders.Update(entity);
        return Task.FromResult(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            _context.PurchaseOrders.Remove(entity);
            await Task.CompletedTask; // Add await to satisfy async requirement
        }
    }
}
