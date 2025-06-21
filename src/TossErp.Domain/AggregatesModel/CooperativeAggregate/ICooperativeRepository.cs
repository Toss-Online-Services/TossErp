using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TossErp.Domain.Enums;
using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.CooperativeAggregate
{
    public interface ICooperativeRepository : IRepository<Cooperative>
    {
        Task<Cooperative> GetByIdAsync(Guid id);
        Task<IEnumerable<Cooperative>> GetAllAsync();
        Task<IEnumerable<Cooperative>> GetByCooperativeTypeAsync(CooperativeType cooperativeType);
        Task<IEnumerable<Cooperative>> GetByTownshipAsync(string township);
        Task<IEnumerable<Cooperative>> GetByProvinceAsync(string province);
        Task<IEnumerable<Cooperative>> GetRegisteredAsync();
        Task<IEnumerable<Cooperative>> GetActiveAsync();
        Task<IEnumerable<Cooperative>> SearchAsync(string searchTerm);
        Task<int> GetActiveMemberCountAsync(Guid cooperativeId);
        Task<decimal> GetTotalShareValueAsync(Guid cooperativeId);
        Task<bool> HasMinimumMembersAsync(Guid cooperativeId, int minimumMembers = 5);
        Task<bool> IsInTownshipAsync(Guid cooperativeId, string townshipName);
        Task<bool> IsInProvinceAsync(Guid cooperativeId, string provinceName);
        Task<int> GetCountByCooperativeTypeAsync(CooperativeType cooperativeType);
        Task<int> GetCountByTownshipAsync(string township);
        Task<int> GetCountByProvinceAsync(string province);
        Task<int> GetRegisteredCountAsync();
        Task<int> GetActiveCountAsync();
        Task<Cooperative> AddAsync(Cooperative cooperative);
        Task UpdateAsync(Cooperative cooperative);
        void Delete(Cooperative cooperative);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByNameAsync(string cooperativeName);
        Task<IEnumerable<string>> GetTownshipsAsync();
        Task<IEnumerable<string>> GetProvincesAsync();
    }
} 
