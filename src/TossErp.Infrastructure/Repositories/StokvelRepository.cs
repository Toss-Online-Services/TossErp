using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Stokvel> AddAsync(Stokvel stokvel)
        {
            var entry = await _context.Stokvels.AddAsync(stokvel);
            return entry.Entity;
        }

        public async Task<Stokvel> GetByIdAsync(Guid id)
        {
            return await _context.Stokvels
                .Include(s => s.Members)
                .Include(s => s.Contributions)
                .Include(s => s.Payouts)
                .Include(s => s.Meetings)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Stokvel>> GetAllAsync()
        {
            return await _context.Stokvels
                .Include(s => s.Members)
                .Include(s => s.Contributions)
                .Include(s => s.Payouts)
                .Include(s => s.Meetings)
                .ToListAsync();
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
                .Where(s => s.StokvelName.Contains(searchTerm) || 
                           s.Description.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<decimal> GetTotalContributionsAsync(Guid stokvelId)
        {
            return await _context.StokvelContributions
                .Where(c => c.StokvelId == stokvelId && c.IsConfirmed)
                .SumAsync(c => c.Amount);
        }

        public async Task<decimal> GetTotalPayoutsAsync(Guid stokvelId)
        {
            return await _context.StokvelPayouts
                .Where(p => p.StokvelId == stokvelId && p.IsProcessed)
                .SumAsync(p => p.Amount);
        }

        public async Task<decimal> GetCurrentBalanceAsync(Guid stokvelId)
        {
            var totalContributions = await GetTotalContributionsAsync(stokvelId);
            var totalPayouts = await GetTotalPayoutsAsync(stokvelId);
            return totalContributions - totalPayouts;
        }

        public async Task<decimal> GetMemberContributionTotalAsync(Guid stokvelId, Guid memberId)
        {
            return await _context.StokvelContributions
                .Where(c => c.StokvelId == stokvelId && c.MemberId == memberId && c.IsConfirmed)
                .SumAsync(c => c.Amount);
        }

        public async Task<decimal> GetMemberPayoutTotalAsync(Guid stokvelId, Guid memberId)
        {
            return await _context.StokvelPayouts
                .Where(p => p.StokvelId == stokvelId && p.MemberId == memberId && p.IsProcessed)
                .SumAsync(p => p.Amount);
        }

        public async Task<decimal> GetMemberBalanceAsync(Guid stokvelId, Guid memberId)
        {
            var contributionTotal = await GetMemberContributionTotalAsync(stokvelId, memberId);
            var payoutTotal = await GetMemberPayoutTotalAsync(stokvelId, memberId);
            return contributionTotal - payoutTotal;
        }

        public async Task<int> GetActiveMemberCountAsync(Guid stokvelId)
        {
            return await _context.StokvelMembers
                .CountAsync(m => m.StokvelId == stokvelId && m.IsActive);
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
            return activeMemberCount >= stokvel.ContributionSettings.MemberLimit;
        }

        public async Task<IEnumerable<StokvelMember>> GetMembersInRotationOrderAsync(Guid stokvelId)
        {
            return await _context.StokvelMembers
                .Where(m => m.StokvelId == stokvelId && m.IsActive)
                .OrderBy(m => m.JoinDate)
                .ToListAsync();
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

        public void Update(Stokvel stokvel)
        {
            _context.Entry(stokvel).State = EntityState.Modified;
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
            return await _context.Stokvels.AnyAsync(s => s.StokvelName == stokvelName);
        }

        public async Task<IEnumerable<string>> GetContributionFrequenciesAsync()
        {
            return await _context.Stokvels
                .Select(s => s.ContributionSettings.Frequency)
                .Where(f => !string.IsNullOrEmpty(f))
                .Distinct()
                .ToListAsync();
        }
    }
} 
