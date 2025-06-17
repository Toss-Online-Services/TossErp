using POS.Domain.AggregatesModel.CustomerAggregate;
using POS.Domain.Repositories;

namespace POS.Domain.Repositories
{
    public interface ICustomerLoyaltyProgramRepository : IRepository<CustomerLoyaltyProgram>
    {
        Task<CustomerLoyaltyProgram?> GetByIdAsync(Guid id);
        Task<CustomerLoyaltyProgram?> GetByMembershipNumberAsync(string membershipNumber, CancellationToken cancellationToken = default);
        Task<IEnumerable<CustomerLoyaltyProgram>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<IEnumerable<CustomerLoyaltyProgram>> GetActiveProgramsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<CustomerLoyaltyProgram>> GetExpiredProgramsAsync(CancellationToken cancellationToken = default);
        Task<CustomerLoyaltyProgram> AddAsync(CustomerLoyaltyProgram program);
        Task UpdateAsync(CustomerLoyaltyProgram program);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByMembershipNumberAsync(string membershipNumber, CancellationToken cancellationToken = default);
        Task<decimal> GetPointsBalanceAsync(Guid programId, CancellationToken cancellationToken = default);
        Task AddPointsAsync(Guid programId, decimal points, string reason, CancellationToken cancellationToken = default);
        Task RedeemPointsAsync(Guid programId, decimal points, string reason, CancellationToken cancellationToken = default);
    }
} 
