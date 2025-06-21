using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TossErp.Domain.AggregatesModel.TownshipEnterpriseAggregate;
using TossErp.Domain.Enums;
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

        public async Task<TownshipEnterprise?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.TownshipEnterprises
                .Include(e => e.Licenses)
                .Include(e => e.Documents)
                .Include(e => e.Contacts)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<TownshipEnterprise>> ListAsync(CancellationToken cancellationToken = default)
        {
            return await _context.TownshipEnterprises
                .Include(e => e.Licenses)
                .Include(e => e.Documents)
                .Include(e => e.Contacts)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(TownshipEnterprise entity, CancellationToken cancellationToken = default)
        {
            await _context.TownshipEnterprises.AddAsync(entity, cancellationToken);
        }

        public void Update(TownshipEnterprise entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(TownshipEnterprise entity)
        {
            _context.TownshipEnterprises.Remove(entity);
        }

        // Legacy method for backward compatibility
        public async Task<TownshipEnterprise> GetByIdAsync(Guid id)
        {
            var result = await GetByIdAsync(id, CancellationToken.None);
            return result ?? throw new InvalidOperationException($"TownshipEnterprise with ID {id} not found.");
        }

        public async Task<IEnumerable<TownshipEnterprise>> GetAllAsync()
        {
            return await ListAsync();
        }

        public async Task<IEnumerable<TownshipEnterprise>> GetByBusinessTypeAsync(string businessType)
        {
            if (Enum.TryParse<BusinessType>(businessType, out var businessTypeEnum))
            {
                return await _context.TownshipEnterprises
                    .Include(e => e.Licenses)
                    .Include(e => e.Documents)
                    .Include(e => e.Contacts)
                    .Where(e => e.BusinessType == businessTypeEnum)
                    .ToListAsync();
            }
            return new List<TownshipEnterprise>();
        }

        public async Task<IEnumerable<TownshipEnterprise>> GetByTownshipAsync(string township)
        {
            return await _context.TownshipEnterprises
                .Include(e => e.Licenses)
                .Include(e => e.Documents)
                .Include(e => e.Contacts)
                .Where(e => e.Address != null && e.Address.Township == township)
                .ToListAsync();
        }

        public async Task<IEnumerable<TownshipEnterprise>> GetByProvinceAsync(string province)
        {
            return await _context.TownshipEnterprises
                .Include(e => e.Licenses)
                .Include(e => e.Documents)
                .Include(e => e.Contacts)
                .Where(e => e.Address != null && e.Address.Province == province)
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

        public async Task<IEnumerable<TownshipEnterprise>> GetByOwnerIdAsync(Guid ownerId)
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
                           (e.TradingName != null && e.TradingName.Contains(searchTerm)) ||
                           (e.BusinessDescription != null && e.BusinessDescription.Contains(searchTerm)))
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

        public async Task<int> GetCountByBusinessTypeAsync(string businessType)
        {
            if (Enum.TryParse<BusinessType>(businessType, out var businessTypeEnum))
            {
                return await _context.TownshipEnterprises
                    .CountAsync(e => e.BusinessType == businessTypeEnum);
            }
            return 0;
        }

        public async Task<int> GetCountByTownshipAsync(string township)
        {
            return await _context.TownshipEnterprises
                .CountAsync(e => e.Address != null && e.Address.Township == township);
        }

        public async Task<int> GetCountByProvinceAsync(string province)
        {
            return await _context.TownshipEnterprises
                .CountAsync(e => e.Address != null && e.Address.Province == province);
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

        public async Task UpdateAsync(TownshipEnterprise enterprise)
        {
            Update(enterprise);
            await Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.TownshipEnterprises.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string businessName)
        {
            return await _context.TownshipEnterprises.AnyAsync(e => e.BusinessName == businessName);
        }

        public async Task<bool> ExistsByRegistrationNumberAsync(string registrationNumber)
        {
            return await _context.TownshipEnterprises.AnyAsync(e => e.RegistrationNumber == registrationNumber);
        }

        public async Task<IEnumerable<string>> GetTownshipsAsync()
        {
            return await _context.TownshipEnterprises
                .Where(e => e.Address != null)
                .Select(e => e.Address.Township)
                .Distinct()
                .Where(t => !string.IsNullOrEmpty(t))
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetProvincesAsync()
        {
            return await _context.TownshipEnterprises
                .Where(e => e.Address != null)
                .Select(e => e.Address.Province)
                .Distinct()
                .Where(p => !string.IsNullOrEmpty(p))
                .ToListAsync();
        }
    }
} 
