using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        // IRepository<T> implementations
        public async Task<Cooperative?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Cooperatives
                .Include(c => c.Members)
                .Include(c => c.Documents)
                .Include(c => c.Meetings)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Cooperative>> ListAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Cooperatives
                .Include(c => c.Members)
                .Include(c => c.Documents)
                .Include(c => c.Meetings)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Cooperative entity, CancellationToken cancellationToken = default)
        {
            await _context.Cooperatives.AddAsync(entity, cancellationToken);
        }

        public void Update(Cooperative entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(Cooperative entity)
        {
            _context.Cooperatives.Remove(entity);
        }

        // ICooperativeRepository implementations
        public async Task<Cooperative> AddAsync(Cooperative cooperative)
        {
            var entry = await _context.Cooperatives.AddAsync(cooperative);
            return entry.Entity;
        }

        // Legacy method for backward compatibility
        public async Task<Cooperative> GetByIdAsync(Guid id)
        {
            var result = await GetByIdAsync(id, CancellationToken.None);
            return result ?? throw new InvalidOperationException($"Cooperative with ID {id} not found.");
        }

        public async Task<IEnumerable<Cooperative>> GetAllAsync()
        {
            return await ListAsync();
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
                .Where(c => c.Address != null && c.Address.Township == township)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cooperative>> GetByProvinceAsync(string province)
        {
            return await _context.Cooperatives
                .Include(c => c.Members)
                .Include(c => c.Documents)
                .Include(c => c.Meetings)
                .Where(c => c.Address != null && c.Address.Province == province)
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
                           (c.TradingName != null && c.TradingName.Contains(searchTerm)) ||
                           (c.Description != null && c.Description.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<int> GetActiveMemberCountAsync(Guid cooperativeId)
        {
            var cooperative = await _context.Cooperatives
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == cooperativeId);

            if (cooperative == null)
                return 0;

            return cooperative.Members.Count(m => m.IsActive);
        }

        public async Task<decimal> GetTotalShareValueAsync(Guid cooperativeId)
        {
            var cooperative = await _context.Cooperatives
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == cooperativeId);

            if (cooperative == null)
                return 0;

            return cooperative.Members
                .Where(m => m.IsActive)
                .Sum(m => m.ShareValue);
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
                .CountAsync(c => c.Address != null && c.Address.Township == township);
        }

        public async Task<int> GetCountByProvinceAsync(string province)
        {
            return await _context.Cooperatives
                .CountAsync(c => c.Address != null && c.Address.Province == province);
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

        public async Task UpdateAsync(Cooperative cooperative)
        {
            Update(cooperative);
            await Task.CompletedTask;
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
                .Where(c => c.Address != null)
                .Select(c => c.Address.Township)
                .Distinct()
                .Where(t => !string.IsNullOrEmpty(t))
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetProvincesAsync()
        {
            return await _context.Cooperatives
                .Where(c => c.Address != null)
                .Select(c => c.Address.Province)
                .Distinct()
                .Where(p => !string.IsNullOrEmpty(p))
                .ToListAsync();
        }
    }
} 
