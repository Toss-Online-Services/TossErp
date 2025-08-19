using TossErp.Accounting.Domain.Entities;
using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Application.Common.Interfaces;

/// <summary>
/// Repository interface for CashbookEntry entities
/// </summary>
public interface ICashbookEntryRepository
{
    Task<CashbookEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CashbookEntry>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CashbookEntry>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CashbookEntry>> GetByCategoryAsync(EntryCategory category, string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CashbookEntry>> GetUnreconciledAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CashbookEntry>> GetByRelatedEntityAsync(string relatedEntityId, string relatedEntityType, CancellationToken cancellationToken = default);
    Task<IEnumerable<CashbookEntry>> GetByTenantAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<CashbookEntry> AddAsync(CashbookEntry entry, CancellationToken cancellationToken = default);
    Task<CashbookEntry> UpdateAsync(CashbookEntry entry, CancellationToken cancellationToken = default);
    Task DeleteAsync(CashbookEntry entry, CancellationToken cancellationToken = default);
}
