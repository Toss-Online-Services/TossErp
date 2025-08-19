using TossErp.Accounting.Application.Common.Interfaces;
using TossErp.Accounting.Domain.Entities;
using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Infrastructure.Persistence.Repositories;

/// <summary>
/// Mock implementation of ICashbookEntryRepository for testing
/// </summary>
public class MockCashbookEntryRepository : ICashbookEntryRepository
{
    private readonly List<CashbookEntry> _entries = new();

    public Task<CashbookEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entry = _entries.FirstOrDefault(e => e.Id == id);
        return Task.FromResult(entry);
    }

    public Task<IEnumerable<CashbookEntry>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var entries = _entries.Where(e => e.AccountId == accountId);
        return Task.FromResult(entries);
    }

    public Task<IEnumerable<CashbookEntry>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, string tenantId, CancellationToken cancellationToken = default)
    {
        var entries = _entries.Where(e => e.TenantId == tenantId && 
                                         e.TransactionDate.Date >= fromDate.Date && 
                                         e.TransactionDate.Date <= toDate.Date);
        return Task.FromResult(entries);
    }

    public Task<IEnumerable<CashbookEntry>> GetByCategoryAsync(EntryCategory category, string tenantId, CancellationToken cancellationToken = default)
    {
        var entries = _entries.Where(e => e.Category == category && e.TenantId == tenantId);
        return Task.FromResult(entries);
    }

    public Task<IEnumerable<CashbookEntry>> GetUnreconciledAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        var entries = _entries.Where(e => !e.IsReconciled && e.TenantId == tenantId);
        return Task.FromResult(entries);
    }

    public Task<IEnumerable<CashbookEntry>> GetByRelatedEntityAsync(string relatedEntityId, string relatedEntityType, CancellationToken cancellationToken = default)
    {
        var entries = _entries.Where(e => e.RelatedEntityId == relatedEntityId && e.RelatedEntityType == relatedEntityType);
        return Task.FromResult(entries);
    }

    public Task<IEnumerable<CashbookEntry>> GetByTenantAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        var entries = _entries.Where(e => e.TenantId == tenantId);
        return Task.FromResult(entries);
    }

    public Task<CashbookEntry> AddAsync(CashbookEntry entry, CancellationToken cancellationToken = default)
    {
        _entries.Add(entry);
        return Task.FromResult(entry);
    }

    public Task<CashbookEntry> UpdateAsync(CashbookEntry entry, CancellationToken cancellationToken = default)
    {
        var existingIndex = _entries.FindIndex(e => e.Id == entry.Id);
        if (existingIndex >= 0)
        {
            _entries[existingIndex] = entry;
        }
        return Task.FromResult(entry);
    }

    public Task DeleteAsync(CashbookEntry entry, CancellationToken cancellationToken = default)
    {
        _entries.RemoveAll(e => e.Id == entry.Id);
        return Task.CompletedTask;
    }
}
