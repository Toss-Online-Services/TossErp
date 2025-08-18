using TossErp.Sales.Domain.Entities;
using TossErp.Sales.Domain.Enums;

namespace TossErp.Sales.Domain.Common;

/// <summary>
/// Repository interface for Till aggregate
/// </summary>
public interface ITillRepository : IRepository<Till>
{
    /// <summary>
    /// Get till by code
    /// </summary>
    Task<Till?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get tills by status
    /// </summary>
    Task<IEnumerable<Till>> GetByStatusAsync(TillStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get tills by location
    /// </summary>
    Task<IEnumerable<Till>> GetByLocationAsync(string location, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get open tills
    /// </summary>
    Task<IEnumerable<Till>> GetOpenTillsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get tills with pagination
    /// </summary>
    Task<IEnumerable<Till>> GetTillsAsync(
        TillStatus? status = null,
        string? location = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get till count by status
    /// </summary>
    Task<long> GetCountByStatusAsync(TillStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if till code exists
    /// </summary>
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
}
