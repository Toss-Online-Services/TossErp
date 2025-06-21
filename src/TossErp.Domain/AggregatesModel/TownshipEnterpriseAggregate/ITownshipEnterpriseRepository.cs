using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.TownshipEnterpriseAggregate
{
    public interface ITownshipEnterpriseRepository : IRepository<TownshipEnterprise>
    {
        Task<TownshipEnterprise> GetByIdAsync(Guid id);
        Task<IEnumerable<TownshipEnterprise>> GetAllAsync();
        Task<IEnumerable<TownshipEnterprise>> GetByBusinessTypeAsync(string businessType);
        Task<IEnumerable<TownshipEnterprise>> GetByTownshipAsync(string township);
        Task<IEnumerable<TownshipEnterprise>> GetByProvinceAsync(string province);
        Task<IEnumerable<TownshipEnterprise>> GetByOwnerIdAsync(Guid ownerId);
        Task<IEnumerable<TownshipEnterprise>> GetRegisteredAsync();
        Task<IEnumerable<TownshipEnterprise>> GetActiveAsync();
        Task<IEnumerable<TownshipEnterprise>> SearchAsync(string searchTerm);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByNameAsync(string businessName);
        Task<bool> ExistsByRegistrationNumberAsync(string registrationNumber);
        Task<int> GetCountByBusinessTypeAsync(string businessType);
        Task<int> GetCountByTownshipAsync(string township);
        Task<int> GetCountByProvinceAsync(string province);
        Task<int> GetActiveCountAsync();
        Task<int> GetRegisteredCountAsync();
        Task<TownshipEnterprise> AddAsync(TownshipEnterprise enterprise);
        Task UpdateAsync(TownshipEnterprise enterprise);
        Task<bool> HasValidLicenseAsync(Guid enterpriseId, string licenseType);
        Task<bool> IsInTownshipAsync(Guid enterpriseId, string townshipName);
        Task<bool> IsInProvinceAsync(Guid enterpriseId, string provinceName);
    }
} 
