using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TossErp.Domain.AggregatesModel.TownshipEnterpriseAggregate;
using TossErp.Domain.SeedWork;
using TossErp.Infrastructure.Data;

namespace TossErp.Infrastructure.Repositories
{
    public class TownshipEnterpriseRepository : ITownshipEnterpriseRepository
    {
        private readonly TossErpDbContext _context;

        public TownshipEnterpriseRepository(TossErpDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<TownshipEnterprise> AddAsync(TownshipEnterprise townshipEnterprise)
        {
            var entry = await _context.TownshipEnterprises.AddAsync(townshipEnterprise);
            return entry.Entity;
        }

        public async Task<TownshipEnterprise> GetByIdAsync(Guid id)
        {
            return await _context.TownshipEnterprises
                .Include(e => e.Licenses)
                .Include(e => e.Documents)
                .Include(e => e.Contacts)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<TownshipEnterprise>> GetAllAsync()
        {
            return await _context.TownshipEnterprises
                .Include(e => e.Licenses)
                .Include(e => e.Documents)
                .Include(e => e.Contacts)
                .ToListAsync();
        }

        public async Task<IEnumerable<TownshipEnterprise>> GetByBusinessTypeAsync(BusinessType businessType)
        {
            return await _context.TownshipEnterprises
                .Include(e => e.Licenses)
                .Include(e => e.Documents)
                .Include(e => e.Contacts)
                .Where(e => e.BusinessType == businessType)
                .ToListAsync();
        }

        public async Task<IEnumerable<TownshipEnterprise>> GetByTownshipAsync(string township)
        {
            return await _context.TownshipEnterprises
                .Include(e => e.Licenses)
                .Include(e => e.Documents)
                .Include(e => e.Contacts)
                .Where(e => e.Address.Township == township)
                .ToListAsync();
        }

        public async Task<IEnumerable<TownshipEnterprise>> GetByProvinceAsync(string province)
        {
            return await _context.TownshipEnterprises
                .Include(e => e.Licenses)
                .Include(e => e.Documents)
                .Include(e => e.Contacts)
                .Where(e => e.Address.Province == province)
                .ToListAsync();
        }

        public async Task<IEnumerable<TownshipEnterprise>> GetRegisteredAsync()
        {
            return await _context.TownshipEnterprises
                .Include(e => e.Licenses)
                .Include(e => e.Documents)
                .Include(e => e.Contacts)
                .Where(e => e.IsRegistered)
                .ToListAsync();
        }

        public async Task<IEnumerable<TownshipEnterprise>> GetActiveAsync()
        {
            return await _context.TownshipEnterprises
                .Include(e => e.Licenses)
                .Include(e => e.Documents)
                .Include(e => e.Contacts)
                .Where(e => e.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<TownshipEnterprise>> GetByOwnerAsync(Guid ownerId)
        {
            return await _context.TownshipEnterprises
                .Include(e => e.Licenses)
                .Include(e => e.Documents)
                .Include(e => e.Contacts)
                .Where(e => e.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TownshipEnterprise>> SearchAsync(string searchTerm)
        {
            return await _context.TownshipEnterprises
                .Include(e => e.Licenses)
                .Include(e => e.Documents)
                .Include(e => e.Contacts)
                .Where(e => e.BusinessName.Contains(searchTerm) || 
                           e.TradingName.Contains(searchTerm) ||
                           e.BusinessDescription.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<bool> HasValidLicenseAsync(Guid enterpriseId, string licenseType)
        {
            var enterprise = await _context.TownshipEnterprises
                .Include(e => e.Licenses)
                .FirstOrDefaultAsync(e => e.Id == enterpriseId);

            if (enterprise == null)
                return false;

            return enterprise.HasValidLicense(licenseType);
        }

        public async Task<bool> IsInTownshipAsync(Guid enterpriseId, string townshipName)
        {
            var enterprise = await _context.TownshipEnterprises
                .FirstOrDefaultAsync(e => e.Id == enterpriseId);

            if (enterprise == null)
                return false;

            return enterprise.IsInTownship(townshipName);
        }

        public async Task<bool> IsInProvinceAsync(Guid enterpriseId, string provinceName)
        {
            var enterprise = await _context.TownshipEnterprises
                .FirstOrDefaultAsync(e => e.Id == enterpriseId);

            if (enterprise == null)
                return false;

            return enterprise.IsInProvince(provinceName);
        }

        public async Task<int> GetCountByBusinessTypeAsync(BusinessType businessType)
        {
            return await _context.TownshipEnterprises
                .CountAsync(e => e.BusinessType == businessType);
        }

        public async Task<int> GetCountByTownshipAsync(string township)
        {
            return await _context.TownshipEnterprises
                .CountAsync(e => e.Address.Township == township);
        }

        public async Task<int> GetCountByProvinceAsync(string province)
        {
            return await _context.TownshipEnterprises
                .CountAsync(e => e.Address.Province == province);
        }

        public async Task<int> GetRegisteredCountAsync()
        {
            return await _context.TownshipEnterprises
                .CountAsync(e => e.IsRegistered);
        }

        public async Task<int> GetActiveCountAsync()
        {
            return await _context.TownshipEnterprises
                .CountAsync(e => e.IsActive);
        }

        public void Update(TownshipEnterprise townshipEnterprise)
        {
            _context.Entry(townshipEnterprise).State = EntityState.Modified;
        }

        public void Delete(TownshipEnterprise townshipEnterprise)
        {
            _context.TownshipEnterprises.Remove(townshipEnterprise);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.TownshipEnterprises.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string businessName)
        {
            return await _context.TownshipEnterprises.AnyAsync(e => e.BusinessName == businessName);
        }

        public async Task<IEnumerable<string>> GetTownshipsAsync()
        {
            return await _context.TownshipEnterprises
                .Select(e => e.Address.Township)
                .Where(t => !string.IsNullOrEmpty(t))
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetProvincesAsync()
        {
            return await _context.TownshipEnterprises
                .Select(e => e.Address.Province)
                .Where(p => !string.IsNullOrEmpty(p))
                .Distinct()
                .ToListAsync();
        }
    }
} 
