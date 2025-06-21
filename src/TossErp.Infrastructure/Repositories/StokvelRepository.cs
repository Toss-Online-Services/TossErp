using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TossErp.Domain.AggregatesModel.StokvelAggregate;
using TossErp.Domain.Enums;
using TossErp.Domain.SeedWork;
using TossErp.Infrastructure.Data;

namespace TossErp.Infrastructure.Repositories
{
    public class StokvelRepository : IStokvelRepository
    {
        private readonly TossErpDbContext _context;

        public StokvelRepository(TossErpDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork => _context;

        // IRepository<T> implementations
        public async Task<Stokvel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Stokvels
                .Include(s => s.Members)
                .Include(s => s.Contributions)
                .Include(s => s.Payouts)
                .Include(s => s.Meetings)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Stokvel>> ListAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Stokvels
                .Include(s => s.Members)
                .Include(s => s.Contributions)
                .Include(s => s.Payouts)
                .Include(s => s.Meetings)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Stokvel entity, CancellationToken cancellationToken = default)
        {
            await _context.Stokvels.AddAsync(entity, cancellationToken);
        }

        public void Update(Stokvel entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(Stokvel entity)
        {
            _context.Stokvels.Remove(entity);
        }

        // IStokvelRepository implementations
        public async Task<Stokvel> AddAsync(Stokvel stokvel)
        {
            var entry = await _context.Stokvels.AddAsync(stokvel);
            return entry.Entity;
        }

        // Legacy method for backward compatibility
        public async Task<Stokvel> GetByIdAsync(Guid id)
        {
            var result = await GetByIdAsync(id, CancellationToken.None);
            return result ?? throw new InvalidOperationException($"Stokvel with ID {id} not found.");
        }

        public async Task<IEnumerable<Stokvel>> GetAllAsync()
        {
            return await ListAsync();
        }

        public async Task<IEnumerable<Stokvel>> GetByStokvelTypeAsync(StokvelType stokvelType)
        {
            return await _context.Stokvels
                .Include(s => s.Members)
                .Include(s => s.Contributions)
                .Include(s => s.Payouts)
                .Include(s => s.Meetings)
                .Where(s => s.StokvelType == stokvelType)
                .ToListAsync();
        }

        public async Task<IEnumerable<Stokvel>> GetByContributionAmountRangeAsync(decimal minAmount, decimal maxAmount)
        {
            return await _context.Stokvels
                .Include(s => s.Members)
                .Include(s => s.Contributions)
                .Include(s => s.Payouts)
                .Include(s => s.Meetings)
                .Where(s => s.ContributionSettings.Amount >= minAmount && s.ContributionSettings.Amount <= maxAmount)
                .ToListAsync();
        }

        public async Task<IEnumerable<Stokvel>> GetByContributionFrequencyAsync(string frequency)
        {
            return await _context.Stokvels
                .Include(s => s.Members)
                .Include(s => s.Contributions)
                .Include(s => s.Payouts)
                .Include(s => s.Meetings)
                .Where(s => s.ContributionSettings.Frequency == frequency)
                .ToListAsync();
        }

        public async Task<IEnumerable<Stokvel>> GetActiveAsync()
        {
            return await _context.Stokvels
                .Include(s => s.Members)
                .Include(s => s.Contributions)
                .Include(s => s.Payouts)
                .Include(s => s.Meetings)
                .Where(s => s.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Stokvel>> SearchAsync(string searchTerm)
        {
            return await _context.Stokvels
                .Include(s => s.Members)
                .Include(s => s.Contributions)
                .Include(s => s.Payouts)
                .Include(s => s.Meetings)
                .Where(s => s.Name.Contains(searchTerm) || 
                           s.Description.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<decimal> GetTotalContributionsAsync(Guid stokvelId)
        {
            var stokvel = await _context.Stokvels
                .Include(s => s.Contributions)
                .FirstOrDefaultAsync(s => s.Id == stokvelId);

            if (stokvel == null)
                return 0;

            return stokvel.Contributions
                .Where(c => c.IsConfirmed)
                .Sum(c => c.Amount);
        }

        public async Task<decimal> GetTotalPayoutsAsync(Guid stokvelId)
        {
            var stokvel = await _context.Stokvels
                .Include(s => s.Payouts)
                .FirstOrDefaultAsync(s => s.Id == stokvelId);

            if (stokvel == null)
                return 0;

            return stokvel.Payouts
                .Where(p => p.IsProcessed)
                .Sum(p => p.Amount);
        }

        public async Task<decimal> GetCurrentBalanceAsync(Guid stokvelId)
        {
            var totalContributions = await GetTotalContributionsAsync(stokvelId);
            var totalPayouts = await GetTotalPayoutsAsync(stokvelId);
            return totalContributions - totalPayouts;
        }

        public async Task<decimal> GetMemberContributionTotalAsync(Guid stokvelId, Guid memberId)
        {
            var stokvel = await _context.Stokvels
                .Include(s => s.Contributions)
                .FirstOrDefaultAsync(s => s.Id == stokvelId);

            if (stokvel == null)
                return 0;

            return stokvel.Contributions
                .Where(c => c.MemberId == memberId && c.IsConfirmed)
                .Sum(c => c.Amount);
        }

        public async Task<decimal> GetMemberPayoutTotalAsync(Guid stokvelId, Guid memberId)
        {
            var stokvel = await _context.Stokvels
                .Include(s => s.Payouts)
                .FirstOrDefaultAsync(s => s.Id == stokvelId);

            if (stokvel == null)
                return 0;

            return stokvel.Payouts
                .Where(p => p.MemberId == memberId && p.IsProcessed)
                .Sum(p => p.Amount);
        }

        public async Task<decimal> GetMemberBalanceAsync(Guid stokvelId, Guid memberId)
        {
            var contributionTotal = await GetMemberContributionTotalAsync(stokvelId, memberId);
            var payoutTotal = await GetMemberPayoutTotalAsync(stokvelId, memberId);
            return contributionTotal - payoutTotal;
        }

        public async Task<int> GetActiveMemberCountAsync(Guid stokvelId)
        {
            var stokvel = await _context.Stokvels
                .Include(s => s.Members)
                .FirstOrDefaultAsync(s => s.Id == stokvelId);

            if (stokvel == null)
                return 0;

            return stokvel.Members.Count(m => m.IsActive);
        }

        public async Task<bool> HasMinimumMembersAsync(Guid stokvelId, int minimumMembers = 5)
        {
            var memberCount = await GetActiveMemberCountAsync(stokvelId);
            return memberCount >= minimumMembers;
        }

        public async Task<bool> IsFullAsync(Guid stokvelId)
        {
            var stokvel = await _context.Stokvels
                .FirstOrDefaultAsync(s => s.Id == stokvelId);

            if (stokvel == null)
                return false;

            var activeMemberCount = await GetActiveMemberCountAsync(stokvelId);
            return activeMemberCount >= stokvel.MaxMembers;
        }

        public async Task<IEnumerable<StokvelMember>> GetMembersInRotationOrderAsync(Guid stokvelId)
        {
            var stokvel = await _context.Stokvels
                .Include(s => s.Members)
                .FirstOrDefaultAsync(s => s.Id == stokvelId);

            if (stokvel == null)
                return new List<StokvelMember>();

            return stokvel.Members
                .Where(m => m.IsActive)
                .OrderBy(m => m.JoinDate)
                .ToList();
        }

        public async Task<int> GetCountByStokvelTypeAsync(StokvelType stokvelType)
        {
            return await _context.Stokvels
                .CountAsync(s => s.StokvelType == stokvelType);
        }

        public async Task<int> GetCountByContributionAmountRangeAsync(decimal minAmount, decimal maxAmount)
        {
            return await _context.Stokvels
                .CountAsync(s => s.ContributionSettings.Amount >= minAmount && s.ContributionSettings.Amount <= maxAmount);
        }

        public async Task<int> GetActiveCountAsync()
        {
            return await _context.Stokvels
                .CountAsync(s => s.IsActive);
        }

        public async Task UpdateAsync(Stokvel stokvel)
        {
            Update(stokvel);
            await Task.CompletedTask;
        }

        public void Delete(Stokvel stokvel)
        {
            _context.Stokvels.Remove(stokvel);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Stokvels.AnyAsync(s => s.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string stokvelName)
        {
            return await _context.Stokvels.AnyAsync(s => s.Name == stokvelName);
        }

        public async Task<IEnumerable<string>> GetContributionFrequenciesAsync()
        {
            return await _context.Stokvels
                .Select(s => s.ContributionSettings.Frequency)
                .Distinct()
                .Where(f => !string.IsNullOrEmpty(f))
                .ToListAsync();
        }
    }
} 
