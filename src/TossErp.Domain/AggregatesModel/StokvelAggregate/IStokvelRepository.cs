using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TossErp.Domain.Enums;
using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.StokvelAggregate
{
    public interface IStokvelRepository : IRepository<Stokvel>
    {
        Task<Stokvel> GetByIdAsync(Guid id);
        Task<IEnumerable<Stokvel>> GetAllAsync();
        Task<IEnumerable<Stokvel>> GetByStokvelTypeAsync(StokvelType stokvelType);
        Task<IEnumerable<Stokvel>> GetByContributionAmountRangeAsync(decimal minAmount, decimal maxAmount);
        Task<IEnumerable<Stokvel>> GetByContributionFrequencyAsync(string frequency);
        Task<IEnumerable<Stokvel>> GetActiveAsync();
        Task<IEnumerable<Stokvel>> SearchAsync(string searchTerm);
        Task<decimal> GetTotalContributionsAsync(Guid stokvelId);
        Task<decimal> GetTotalPayoutsAsync(Guid stokvelId);
        Task<decimal> GetCurrentBalanceAsync(Guid stokvelId);
        Task<decimal> GetMemberContributionTotalAsync(Guid stokvelId, Guid memberId);
        Task<decimal> GetMemberPayoutTotalAsync(Guid stokvelId, Guid memberId);
        Task<decimal> GetMemberBalanceAsync(Guid stokvelId, Guid memberId);
        Task<int> GetActiveMemberCountAsync(Guid stokvelId);
        Task<bool> HasMinimumMembersAsync(Guid stokvelId, int minimumMembers = 5);
        Task<bool> IsFullAsync(Guid stokvelId);
        Task<IEnumerable<StokvelMember>> GetMembersInRotationOrderAsync(Guid stokvelId);
        Task<int> GetCountByStokvelTypeAsync(StokvelType stokvelType);
        Task<int> GetCountByContributionAmountRangeAsync(decimal minAmount, decimal maxAmount);
        Task<int> GetActiveCountAsync();
        Task<Stokvel> AddAsync(Stokvel stokvel);
        Task UpdateAsync(Stokvel stokvel);
        void Delete(Stokvel stokvel);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByNameAsync(string stokvelName);
        Task<IEnumerable<string>> GetContributionFrequenciesAsync();
    }
} 
