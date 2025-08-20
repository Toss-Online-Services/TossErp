using Crm.Domain.Entities;
using Crm.Domain.Repositories;

namespace Crm.Infrastructure.Repositories;

public class InMemoryLoyaltyTransactionRepository : ILoyaltyTransactionRepository
{
    private readonly List<LoyaltyTransaction> _transactions = new();
    private readonly object _lock = new();

    public Task<LoyaltyTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var transaction = _transactions.FirstOrDefault(t => t.Id == id);
            return Task.FromResult(transaction);
        }
    }

    public Task<IEnumerable<LoyaltyTransaction>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var transactions = _transactions.Where(t => t.CustomerId == customerId);
            return Task.FromResult(transactions);
        }
    }

    public Task<IEnumerable<LoyaltyTransaction>> GetByTypeAsync(LoyaltyTransactionType type, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var transactions = _transactions.Where(t => t.Type == type);
            return Task.FromResult(transactions);
        }
    }

    public Task<IEnumerable<LoyaltyTransaction>> GetExpiredTransactionsAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var transactions = _transactions.Where(t => t.IsExpired);
            return Task.FromResult(transactions);
        }
    }

    public Task<int> GetCustomerBalanceAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var balance = _transactions
                .Where(t => t.CustomerId == customerId && !t.IsExpired)
                .Sum(t => t.Points);
            return Task.FromResult(balance);
        }
    }

    public Task AddAsync(LoyaltyTransaction transaction, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _transactions.Add(transaction);
            return Task.CompletedTask;
        }
    }

    public Task UpdateAsync(LoyaltyTransaction transaction, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var existingTransaction = _transactions.FirstOrDefault(t => t.Id == transaction.Id);
            if (existingTransaction != null)
            {
                var index = _transactions.IndexOf(existingTransaction);
                _transactions[index] = transaction;
            }
            return Task.CompletedTask;
        }
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var transaction = _transactions.FirstOrDefault(t => t.Id == id);
            if (transaction != null)
            {
                _transactions.Remove(transaction);
            }
            return Task.CompletedTask;
        }
    }
}
