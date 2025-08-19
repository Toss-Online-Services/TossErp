using TossErp.Accounting.Domain.Entities;

namespace TossErp.Accounting.Application.Common.Interfaces;

/// <summary>
/// Repository interface for Cashbook entities
/// </summary>
public interface ICashbookRepository
{
    Task<Cashbook?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Cashbook?> GetByNameAsync(string name, string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cashbook>> GetAllAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<Cashbook> AddAsync(Cashbook cashbook, CancellationToken cancellationToken = default);
    Task<Cashbook> UpdateAsync(Cashbook cashbook, CancellationToken cancellationToken = default);
    Task DeleteAsync(Cashbook cashbook, CancellationToken cancellationToken = default);
}

