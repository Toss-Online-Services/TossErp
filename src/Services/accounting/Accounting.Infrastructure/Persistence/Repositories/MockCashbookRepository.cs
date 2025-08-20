using TossErp.Accounting.Application.Common.Interfaces;
using TossErp.Accounting.Domain.Common;
using TossErp.Accounting.Domain.Entities;

namespace TossErp.Accounting.Infrastructure.Persistence.Repositories;

/// <summary>
/// Mock implementation of ICashbookRepository for testing
/// </summary>
public class MockCashbookRepository : ICashbookRepository
{
    private readonly List<Cashbook> _cashbooks = new();

    public Task<Cashbook?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cashbook = _cashbooks.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(cashbook);
    }

    public Task<Cashbook?> GetByNameAsync(string name, string tenantId, CancellationToken cancellationToken = default)
    {
        var cashbook = _cashbooks.FirstOrDefault(c => c.Name == name && c.TenantId == tenantId);
        return Task.FromResult(cashbook);
    }

    public Task<IEnumerable<Cashbook>> GetAllAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        var cashbooks = _cashbooks.Where(c => c.TenantId == tenantId);
        return Task.FromResult(cashbooks);
    }

    public Task<Cashbook> AddAsync(Cashbook cashbook, CancellationToken cancellationToken = default)
    {
        _cashbooks.Add(cashbook);
        return Task.FromResult(cashbook);
    }

    public Task<Cashbook> UpdateAsync(Cashbook cashbook, CancellationToken cancellationToken = default)
    {
        var existingIndex = _cashbooks.FindIndex(c => c.Id == cashbook.Id);
        if (existingIndex >= 0)
        {
            _cashbooks[existingIndex] = cashbook;
        }
        return Task.FromResult(cashbook);
    }

    public Task DeleteAsync(Cashbook cashbook, CancellationToken cancellationToken = default)
    {
        _cashbooks.RemoveAll(c => c.Id == cashbook.Id);
        return Task.CompletedTask;
    }
}


