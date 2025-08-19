using TossErp.Accounting.Application.Common.Interfaces;

namespace TossErp.Accounting.Infrastructure.Persistence;

/// <summary>
/// Mock implementation of IUnitOfWork for testing
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    public IAccountRepository Accounts { get; }
    public ICashbookRepository Cashbooks { get; }
    public ICashbookEntryRepository CashbookEntries { get; }
    public IStockValuationSnapshotRepository StockValuationSnapshots { get; }

    public UnitOfWork(
        IAccountRepository accountRepository,
        ICashbookRepository cashbookRepository,
        ICashbookEntryRepository cashbookEntryRepository,
        IStockValuationSnapshotRepository stockValuationSnapshotRepository)
    {
        Accounts = accountRepository;
        Cashbooks = cashbookRepository;
        CashbookEntries = cashbookEntryRepository;
        StockValuationSnapshots = stockValuationSnapshotRepository;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Mock implementation - just return 1 to indicate success
        return Task.FromResult(1);
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        // Mock implementation - no actual transaction needed
        return Task.CompletedTask;
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        // Mock implementation - no actual transaction needed
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        // Mock implementation - no actual transaction needed
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        // Mock implementation - no cleanup needed
    }
}
