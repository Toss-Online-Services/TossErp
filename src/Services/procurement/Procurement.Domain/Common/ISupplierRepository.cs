using TossErp.Procurement.Domain.Entities;
using TossErp.Procurement.Domain.Enums;

namespace TossErp.Procurement.Domain.Common;

/// <summary>
/// Repository interface for Supplier entity
/// </summary>
public interface ISupplierRepository : IRepository<Supplier, Guid>
{
    /// <summary>
    /// Get suppliers by status
    /// </summary>
    /// <param name="status">Supplier status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Suppliers with specified status</returns>
    Task<IEnumerable<Supplier>> GetByStatusAsync(SupplierStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get supplier by code
    /// </summary>
    /// <param name="code">Supplier code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Supplier with specified code</returns>
    Task<Supplier?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get suppliers by name (partial match)
    /// </summary>
    /// <param name="name">Supplier name (partial)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Suppliers matching name</returns>
    Task<IEnumerable<Supplier>> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active suppliers
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Active suppliers</returns>
    Task<IEnumerable<Supplier>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get suppliers pending approval
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Suppliers pending approval</returns>
    Task<IEnumerable<Supplier>> GetPendingApprovalAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if supplier code exists
    /// </summary>
    /// <param name="code">Supplier code</param>
    /// <param name="excludeId">Exclude supplier ID (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if code exists</returns>
    Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
