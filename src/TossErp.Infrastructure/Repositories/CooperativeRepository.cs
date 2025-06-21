using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TossErp.Domain.AggregatesModel.CooperativeAggregate;
using TossErp.Domain.Enums;
using TossErp.Domain.SeedWork;
using TossErp.Infrastructure.Data;

namespace TossErp.Infrastructure.Repositories
{
    public class CooperativeRepository : ICooperativeRepository
    {
        private readonly TossErpDbContext _context;

        public CooperativeRepository(TossErpDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Cooperative> AddAsync(Cooperative cooperative)
        {
            var entry = await _context.Cooperatives.AddAsync(cooperative);
            return entry.Entity;
        }

        public async Task<Cooperative> GetByIdAsync(Guid id)
        {
            return await _context.Cooperatives
                .Include(c => c.Members)
                .Include(c => c.Documents)
                .Include(c => c.Meetings)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Cooperative>> GetAllAsync()
        {
            return await _context.Cooperatives
                .Include(c => c.Members)
                .Include(c => c.Documents)
                .Include(c => c.Meetings)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cooperative>> GetByCooperativeTypeAsync(CooperativeType cooperativeType)
        {
            return await _context.Cooperatives
                .Include(c => c.Members)
                .Include(c => c.Documents)
                .Include(c => c.Meetings)
                .Where(c => c.CooperativeType == cooperativeType)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cooperative>> GetByTownshipAsync(string township)
        {
            return await _context.Cooperatives
                .Include(c => c.Members)
                .Include(c => c.Documents)
                .Include(c => c.Meetings)
                .Where(c => c.Address.Township == township)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cooperative>> GetByProvinceAsync(string province)
        {
            return await _context.Cooperatives
                .Include(c => c.Members)
                .Include(c => c.Documents)
                .Include(c => c.Meetings)
                .Where(c => c.Address.Province == province)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cooperative>> GetRegisteredAsync()
        {
            return await _context.Cooperatives
                .Include(c => c.Members)
                .Include(c => c.Documents)
                .Include(c => c.Meetings)
                .Where(c => c.IsRegistered)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cooperative>> GetActiveAsync()
        {
            return await _context.Cooperatives
                .Include(c => c.Members)
                .Include(c => c.Documents)
                .Include(c => c.Meetings)
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cooperative>> SearchAsync(string searchTerm)
        {
            return await _context.Cooperatives
                .Include(c => c.Members)
                .Include(c => c.Documents)
                .Include(c => c.Meetings)
                .Where(c => c.CooperativeName.Contains(searchTerm) || 
                           c.TradingName.Contains(searchTerm) ||
                           c.Description.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<int> GetActiveMemberCountAsync(Guid cooperativeId)
        {
            return await _context.CooperativeMembers
                .CountAsync(m => m.CooperativeId == cooperativeId && m.IsActive);
        }

        public async Task<decimal> GetTotalShareValueAsync(Guid cooperativeId)
        {
            return await _context.CooperativeMembers
                .Where(m => m.CooperativeId == cooperativeId && m.IsActive)
                .SumAsync(m => m.ShareValue ?? 0);
        }

        public async Task<bool> HasMinimumMembersAsync(Guid cooperativeId, int minimumMembers = 5)
        {
            var memberCount = await GetActiveMemberCountAsync(cooperativeId);
            return memberCount >= minimumMembers;
        }

        public async Task<bool> IsInTownshipAsync(Guid cooperativeId, string townshipName)
        {
            var cooperative = await _context.Cooperatives
                .FirstOrDefaultAsync(c => c.Id == cooperativeId);

            if (cooperative == null)
                return false;

            return cooperative.IsInTownship(townshipName);
        }

        public async Task<bool> IsInProvinceAsync(Guid cooperativeId, string provinceName)
        {
            var cooperative = await _context.Cooperatives
                .FirstOrDefaultAsync(c => c.Id == cooperativeId);

            if (cooperative == null)
                return false;

            return cooperative.IsInProvince(provinceName);
        }

        public async Task<int> GetCountByCooperativeTypeAsync(CooperativeType cooperativeType)
        {
            return await _context.Cooperatives
                .CountAsync(c => c.CooperativeType == cooperativeType);
        }

        public async Task<int> GetCountByTownshipAsync(string township)
        {
            return await _context.Cooperatives
                .CountAsync(c => c.Address.Township == township);
        }

        public async Task<int> GetCountByProvinceAsync(string province)
        {
            return await _context.Cooperatives
                .CountAsync(c => c.Address.Province == province);
        }

        public async Task<int> GetRegisteredCountAsync()
        {
            return await _context.Cooperatives
                .CountAsync(c => c.IsRegistered);
        }

        public async Task<int> GetActiveCountAsync()
        {
            return await _context.Cooperatives
                .CountAsync(c => c.IsActive);
        }

        public void Update(Cooperative cooperative)
        {
            _context.Entry(cooperative).State = EntityState.Modified;
        }

        public void Delete(Cooperative cooperative)
        {
            _context.Cooperatives.Remove(cooperative);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Cooperatives.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string cooperativeName)
        {
            return await _context.Cooperatives.AnyAsync(c => c.CooperativeName == cooperativeName);
        }

        public async Task<IEnumerable<string>> GetTownshipsAsync()
        {
            return await _context.Cooperatives
                .Select(c => c.Address.Township)
                .Where(t => !string.IsNullOrEmpty(t))
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetProvincesAsync()
        {
            return await _context.Cooperatives
                .Select(c => c.Address.Province)
                .Where(p => !string.IsNullOrEmpty(p))
                .Distinct()
                .ToListAsync();
        }
    }
} 
