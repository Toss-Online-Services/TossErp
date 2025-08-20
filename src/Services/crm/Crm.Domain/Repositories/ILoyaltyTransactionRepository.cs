namespace Crm.Domain.Repositories;

public interface ILoyaltyTransactionRepository
{
    Task<LoyaltyTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoyaltyTransaction>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoyaltyTransaction>> GetByTypeAsync(LoyaltyTransactionType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoyaltyTransaction>> GetExpiredTransactionsAsync(CancellationToken cancellationToken = default);
    Task<int> GetCustomerBalanceAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(LoyaltyTransaction transaction, CancellationToken cancellationToken = default);
    Task UpdateAsync(LoyaltyTransaction transaction, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
