using TossErp.Procurement.Domain.Common;
using TossErp.Procurement.Domain.Entities;
using TossErp.Procurement.Domain.Enums;

namespace TossErp.Procurement.API.Services;

/// <summary>
/// Mock implementation of ISupplierRepository for MVP
/// </summary>
public class MockSupplierRepository : ISupplierRepository
{
    private readonly ILogger<MockSupplierRepository> _logger;
    private readonly List<Supplier> _suppliers = new();

    public MockSupplierRepository(ILogger<MockSupplierRepository> logger)
    {
        _logger = logger;
        InitializeSampleData();
    }

    public async Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockSupplierRepository.GetByIdAsync called with ID: {Id}", id);
        return await Task.FromResult(_suppliers.FirstOrDefault(s => s.Id == id));
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockSupplierRepository.GetAllAsync called");
        return await Task.FromResult(_suppliers.AsEnumerable());
    }

    public async Task<IEnumerable<Supplier>> GetByStatusAsync(SupplierStatus status, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockSupplierRepository.GetByStatusAsync called with status: {Status}", status);
        return await Task.FromResult(_suppliers.Where(s => s.Status == status));
    }

    public async Task<Supplier?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockSupplierRepository.GetByCodeAsync called with code: {Code}", code);
        return await Task.FromResult(_suppliers.FirstOrDefault(s => s.Code == code));
    }

    public async Task<IEnumerable<Supplier>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockSupplierRepository.GetByNameAsync called with name: {Name}", name);
        return await Task.FromResult(_suppliers.Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase)));
    }

    public async Task<IEnumerable<Supplier>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockSupplierRepository.GetActiveAsync called");
        return await Task.FromResult(_suppliers.Where(s => s.Status == SupplierStatus.Active));
    }

    public async Task<IEnumerable<Supplier>> GetPendingApprovalAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockSupplierRepository.GetPendingApprovalAsync called");
        return await Task.FromResult(_suppliers.Where(s => s.Status == SupplierStatus.PendingApproval));
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockSupplierRepository.CodeExistsAsync called with code: {Code}, excludeId: {ExcludeId}", code, excludeId);
        return await Task.FromResult(_suppliers.Any(s => s.Code == code && (!excludeId.HasValue || s.Id != excludeId.Value)));
    }

    public async Task<Supplier> AddAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockSupplierRepository.AddAsync called for supplier: {Name} ({Code})", supplier.Name, supplier.Code);
        _suppliers.Add(supplier);
        return await Task.FromResult(supplier);
    }

    public async Task<Supplier> UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockSupplierRepository.UpdateAsync called for supplier: {Name} ({Code})", supplier.Name, supplier.Code);
        var existingIndex = _suppliers.FindIndex(s => s.Id == supplier.Id);
        if (existingIndex >= 0)
        {
            _suppliers[existingIndex] = supplier;
        }
        return await Task.FromResult(supplier);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockSupplierRepository.DeleteAsync called for ID: {Id}", id);
        var supplier = _suppliers.FirstOrDefault(s => s.Id == id);
        if (supplier != null)
        {
            _suppliers.Remove(supplier);
        }
        await Task.CompletedTask;
    }

    private void InitializeSampleData()
    {
        // Create sample suppliers
        var supplier1 = Supplier.Create("ABC Electronics", "ABC001", "default-tenant");
        supplier1.UpdateContactInfo("John Smith", "john.smith@abcelectronics.com", "+1-555-0123");
        supplier1.UpdateBusinessInfo("123 Main St", "New York", "NY", "10001", "USA", "TAX123456");
        supplier1.UpdateFinancialInfo("Bank of America", "1234567890", "021000021", "Net 30");
        supplier1.UpdateOperationalInfo(14.0m, "system");
        supplier1.Activate();
        _suppliers.Add(supplier1);

        var supplier2 = Supplier.Create("XYZ Manufacturing", "XYZ002", "default-tenant");
        supplier2.UpdateContactInfo("Jane Doe", "jane.doe@xyzmanufacturing.com", "+1-555-0456");
        supplier2.UpdateBusinessInfo("456 Industrial Ave", "Chicago", "IL", "60601", "USA", "TAX654321");
        supplier2.UpdateFinancialInfo("Chase Bank", "0987654321", "021000021", "Net 45");
        supplier2.UpdateOperationalInfo(21.0m, "system");
        supplier2.Activate();
        _suppliers.Add(supplier2);

        var supplier3 = Supplier.Create("Global Parts Co", "GPC003", "default-tenant");
        supplier3.UpdateContactInfo("Bob Johnson", "bob.johnson@globalparts.com", "+1-555-0789");
        supplier3.UpdateBusinessInfo("789 Warehouse Blvd", "Los Angeles", "CA", "90001", "USA", "TAX987654");
        supplier3.UpdateFinancialInfo("Wells Fargo", "1122334455", "121000248", "Net 60");
        supplier3.UpdateOperationalInfo(30.0m, "system");
        supplier3.Activate();
        _suppliers.Add(supplier3);

        var supplier4 = Supplier.Create("Pending Supplier", "PEND004", "default-tenant");
        supplier4.UpdateContactInfo("Alice Brown", "alice.brown@pending.com", "+1-555-0321");
        supplier4.UpdateBusinessInfo("321 Test St", "Test City", "TX", "12345", "USA", "TAX111111");
        // This supplier remains in PendingApproval status
        _suppliers.Add(supplier4);
    }
}
