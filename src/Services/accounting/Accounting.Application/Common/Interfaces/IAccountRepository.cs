using TossErp.Accounting.Domain.Entities;
using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Application.Common.Interfaces;

/// <summary>
/// Repository interface for Account entities
/// </summary>
public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Account?> GetByCodeAsync(string code, string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Account>> GetByTypeAsync(AccountType type, string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Account>> GetByCategoryAsync(AccountCategory category, string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Account>> GetAllAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<Account> AddAsync(Account account, CancellationToken cancellationToken = default);
    Task<Account> UpdateAsync(Account account, CancellationToken cancellationToken = default);
    Task DeleteAsync(Account account, CancellationToken cancellationToken = default);
}


