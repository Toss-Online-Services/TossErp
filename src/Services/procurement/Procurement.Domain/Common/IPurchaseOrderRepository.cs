using TossErp.Procurement.Domain.Entities;
using TossErp.Procurement.Domain.Enums;

namespace TossErp.Procurement.Domain.Common;

/// <summary>
/// Repository interface for PurchaseOrder aggregate
/// </summary>
public interface IPurchaseOrderRepository : IRepository<PurchaseOrder, Guid>
{
    /// <summary>
    /// Get purchase orders by status
    /// </summary>
    /// <param name="status">Purchase order status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Purchase orders with specified status</returns>
    Task<IEnumerable<PurchaseOrder>> GetByStatusAsync(PurchaseOrderStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get purchase orders by supplier
    /// </summary>
    /// <param name="supplierId">Supplier ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Purchase orders for the supplier</returns>
    Task<IEnumerable<PurchaseOrder>> GetBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get purchase orders by date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Purchase orders within date range</returns>
    Task<IEnumerable<PurchaseOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get purchase orders by purchase order number
    /// </summary>
    /// <param name="purchaseOrderNumber">Purchase order number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Purchase order with specified number</returns>
    Task<PurchaseOrder?> GetByPurchaseOrderNumberAsync(string purchaseOrderNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get purchase orders pending approval
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Purchase orders pending approval</returns>
    Task<IEnumerable<PurchaseOrder>> GetPendingApprovalAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get purchase orders pending receipt
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Purchase orders pending receipt</returns>
    Task<IEnumerable<PurchaseOrder>> GetPendingReceiptAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get overdue purchase orders
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Overdue purchase orders</returns>
    Task<IEnumerable<PurchaseOrder>> GetOverdueAsync(CancellationToken cancellationToken = default);
}
