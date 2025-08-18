using TossErp.Procurement.Domain.Common;
using TossErp.Procurement.Domain.Entities;
using TossErp.Procurement.Domain.Enums;
using TossErp.Procurement.Domain.ValueObjects;

namespace TossErp.Procurement.API.Services;

/// <summary>
/// Mock implementation of IPurchaseOrderRepository for MVP
/// </summary>
public class MockPurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly ILogger<MockPurchaseOrderRepository> _logger;
    private readonly List<PurchaseOrder> _purchaseOrders = new();

    public MockPurchaseOrderRepository(ILogger<MockPurchaseOrderRepository> logger)
    {
        _logger = logger;
        InitializeSampleData();
    }

    public async Task<PurchaseOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockPurchaseOrderRepository.GetByIdAsync called with ID: {Id}", id);
        return await Task.FromResult(_purchaseOrders.FirstOrDefault(po => po.Id == id));
    }

    public async Task<IEnumerable<PurchaseOrder>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockPurchaseOrderRepository.GetAllAsync called");
        return await Task.FromResult(_purchaseOrders.AsEnumerable());
    }

    public async Task<IEnumerable<PurchaseOrder>> GetByStatusAsync(PurchaseOrderStatus status, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockPurchaseOrderRepository.GetByStatusAsync called with status: {Status}", status);
        return await Task.FromResult(_purchaseOrders.Where(po => po.Status == status));
    }

    public async Task<IEnumerable<PurchaseOrder>> GetBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockPurchaseOrderRepository.GetBySupplierAsync called with supplier ID: {SupplierId}", supplierId);
        return await Task.FromResult(_purchaseOrders.Where(po => po.SupplierId == supplierId));
    }

    public async Task<IEnumerable<PurchaseOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockPurchaseOrderRepository.GetByDateRangeAsync called from {StartDate} to {EndDate}", startDate, endDate);
        return await Task.FromResult(_purchaseOrders.Where(po => po.OrderDate >= startDate && po.OrderDate <= endDate));
    }

    public async Task<PurchaseOrder?> GetByPurchaseOrderNumberAsync(string purchaseOrderNumber, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockPurchaseOrderRepository.GetByPurchaseOrderNumberAsync called with number: {Number}", purchaseOrderNumber);
        return await Task.FromResult(_purchaseOrders.FirstOrDefault(po => po.PurchaseOrderNumber.Value == purchaseOrderNumber));
    }

    public async Task<IEnumerable<PurchaseOrder>> GetPendingApprovalAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockPurchaseOrderRepository.GetPendingApprovalAsync called");
        return await Task.FromResult(_purchaseOrders.Where(po => po.Status == PurchaseOrderStatus.Submitted));
    }

    public async Task<IEnumerable<PurchaseOrder>> GetPendingReceiptAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockPurchaseOrderRepository.GetPendingReceiptAsync called");
        return await Task.FromResult(_purchaseOrders.Where(po => 
            po.Status == PurchaseOrderStatus.Acknowledged || 
            po.Status == PurchaseOrderStatus.PartiallyReceived));
    }

    public async Task<IEnumerable<PurchaseOrder>> GetOverdueAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockPurchaseOrderRepository.GetOverdueAsync called");
        var now = DateTime.UtcNow;
        return await Task.FromResult(_purchaseOrders.Where(po => 
            po.ExpectedDeliveryDate.HasValue && 
            po.ExpectedDeliveryDate.Value < now && 
            po.Status != PurchaseOrderStatus.Received &&
            po.Status != PurchaseOrderStatus.Cancelled));
    }

    public async Task<PurchaseOrder> AddAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockPurchaseOrderRepository.AddAsync called for PO: {PurchaseOrderNumber}", purchaseOrder.PurchaseOrderNumber.Value);
        _purchaseOrders.Add(purchaseOrder);
        return await Task.FromResult(purchaseOrder);
    }

    public async Task<PurchaseOrder> UpdateAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockPurchaseOrderRepository.UpdateAsync called for PO: {PurchaseOrderNumber}", purchaseOrder.PurchaseOrderNumber.Value);
        var existingIndex = _purchaseOrders.FindIndex(po => po.Id == purchaseOrder.Id);
        if (existingIndex >= 0)
        {
            _purchaseOrders[existingIndex] = purchaseOrder;
        }
        return await Task.FromResult(purchaseOrder);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockPurchaseOrderRepository.DeleteAsync called for ID: {Id}", id);
        var purchaseOrder = _purchaseOrders.FirstOrDefault(po => po.Id == id);
        if (purchaseOrder != null)
        {
            _purchaseOrders.Remove(purchaseOrder);
        }
        await Task.CompletedTask;
    }

    private void InitializeSampleData()
    {
        // Create sample purchase orders
        var supplierId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        // Sample PO 1
        var po1 = PurchaseOrder.Create(supplierId, "Sample Supplier 1", "default-tenant", PaymentTerms.Net30, "PO-2024-000001");
        po1.AddItem(itemId, "Sample Item 1", "ITEM001", 10, 25.00m, 0.15m);
        po1.AddItem(Guid.NewGuid(), "Sample Item 2", "ITEM002", 5, 50.00m, 0.15m, 10.0m);
        po1.Submit("system");
        _purchaseOrders.Add(po1);

        // Sample PO 2
        var po2 = PurchaseOrder.Create(supplierId, "Sample Supplier 1", "default-tenant", PaymentTerms.Net15, "PO-2024-000002");
        po2.AddItem(itemId, "Sample Item 1", "ITEM001", 20, 25.00m, 0.15m);
        po2.Submit("system");
        po2.Approve("manager", "Approved for urgent delivery");
        po2.Send("procurement");
        _purchaseOrders.Add(po2);

        // Sample PO 3
        var po3 = PurchaseOrder.Create(Guid.NewGuid(), "Sample Supplier 2", "default-tenant", PaymentTerms.Net30, "PO-2024-000003");
        po3.AddItem(Guid.NewGuid(), "Sample Item 3", "ITEM003", 15, 75.00m, 0.15m);
        po3.Submit("system");
        po3.Approve("manager");
        po3.Send("procurement");
        po3.Acknowledge("supplier");
        po3.ReceiveItems(itemId, 5, "warehouse");
        _purchaseOrders.Add(po3);
    }
}
