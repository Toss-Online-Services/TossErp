using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.Repositories;
using Crm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Crm.Infrastructure.Persistence.Repositories;

public class LeadRepository : ILeadRepository
{
    private readonly CrmDbContext _context;
    private readonly ILogger<LeadRepository> _logger;

    public LeadRepository(CrmDbContext context, ILogger<LeadRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Lead?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Leads
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lead with ID: {LeadId}", id);
            throw;
        }
    }

    public async Task<Lead?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Leads
                .FirstOrDefaultAsync(l => l.Email.Value == email && !l.IsDeleted, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lead with email: {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<Lead>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Leads
                .Where(l => !l.IsDeleted)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all leads");
            throw;
        }
    }

    public async Task<IEnumerable<Lead>> GetByStatusAsync(LeadStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Leads
                .Where(l => l.Status == status && !l.IsDeleted)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leads by status: {Status}", status);
            throw;
        }
    }

    public async Task<IEnumerable<Lead>> GetBySourceAsync(LeadSource source, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Leads
                .Where(l => l.Source == source && !l.IsDeleted)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leads by source: {Source}", source);
            throw;
        }
    }

    public async Task<IEnumerable<Lead>> GetByScoreRangeAsync(int minScore, int maxScore, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Leads
                .Where(l => l.Score.Value >= minScore && l.Score.Value <= maxScore && !l.IsDeleted)
                .OrderByDescending(l => l.Score.Value)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leads by score range: {MinScore} - {MaxScore}", minScore, maxScore);
            throw;
        }
    }

    public async Task<IEnumerable<Lead>> GetQualifiedLeadsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Leads
                .Where(l => l.Status == LeadStatus.Qualified && !l.IsDeleted)
                .OrderByDescending(l => l.Score.Value)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving qualified leads");
            throw;
        }
    }

    public async Task<IEnumerable<Lead>> GetHotLeadsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Leads
                .Where(l => l.Score.Value >= 75 && 
                           l.Status != LeadStatus.Converted && 
                           l.Status != LeadStatus.Lost && 
                           l.Status != LeadStatus.DoNotContact && 
                           !l.IsDeleted)
                .OrderByDescending(l => l.Score.Value)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hot leads");
            throw;
        }
    }

    public async Task<IEnumerable<Lead>> GetByIndustryAsync(string industry, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Leads
                .Where(l => l.Industry == industry && !l.IsDeleted)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leads by industry: {Industry}", industry);
            throw;
        }
    }

    public async Task<IEnumerable<Lead>> GetByCampaignAsync(string campaignName, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Leads
                .Where(l => l.CampaignName == campaignName && !l.IsDeleted)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leads by campaign: {CampaignName}", campaignName);
            throw;
        }
    }

    public async Task<IEnumerable<Lead>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            var term = searchTerm.ToLower();
            return await _context.Leads
                .Where(l => !l.IsDeleted && 
                    (l.FirstName.ToLower().Contains(term) ||
                     l.LastName.ToLower().Contains(term) ||
                     l.Company.ToLower().Contains(term) ||
                     l.Email.Value.ToLower().Contains(term) ||
                     (l.JobTitle != null && l.JobTitle.ToLower().Contains(term))))
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching leads with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<Lead> AddAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Leads.Add(lead);
            await _context.SaveChangesAsync(cancellationToken);
            return lead;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding lead: {LeadId}", lead.Id);
            throw;
        }
    }

    public async Task UpdateAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Leads.Update(lead);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating lead: {LeadId}", lead.Id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var lead = await GetByIdAsync(id, cancellationToken);
            if (lead != null)
            {
                lead.Delete("System");
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting lead: {LeadId}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Leads
                .AnyAsync(l => l.Id == id && !l.IsDeleted, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if lead exists: {LeadId}", id);
            throw;
        }
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Leads
                .AnyAsync(l => l.Email.Value == email && !l.IsDeleted, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if email exists: {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<Lead>> GetByAssignedUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Leads
                .Where(l => l.AssignedTo == userId && !l.IsDeleted)
                .OrderByDescending(l => l.Score.Value)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leads by assigned user: {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<Lead>> GetStaleLeadsAsync(int daysThreshold = 30, CancellationToken cancellationToken = default)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysThreshold);
            return await _context.Leads
                .Where(l => l.ModifiedAt < cutoffDate && 
                           l.Status != LeadStatus.Converted && 
                           l.Status != LeadStatus.Lost && 
                           l.Status != LeadStatus.DoNotContact && 
                           !l.IsDeleted)
                .OrderBy(l => l.ModifiedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving stale leads with threshold: {DaysThreshold}", daysThreshold);
            throw;
        }
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Leads
                .CountAsync(l => !l.IsDeleted, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lead count");
            throw;
        }
    }

    public async Task<int> GetCountByStatusAsync(LeadStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Leads
                .CountAsync(l => l.Status == status && !l.IsDeleted, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lead count by status: {Status}", status);
            throw;
        }
    }

    public async Task<IEnumerable<Lead>> GetByAssigneeAsync(string assignedTo, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Leads
                .Where(l => l.AssignedTo == assignedTo && !l.IsDeleted)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting leads by assignee: {AssignedTo}", assignedTo);
            throw;
        }
    }

    public async Task DeleteAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        try
        {
            lead.Delete("system");
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting lead: {LeadId}", lead.Id);
            throw;
        }
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Leads
                .AnyAsync(l => l.Email.Value == email && !l.IsDeleted, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if lead email exists: {Email}", email);
            throw;
        }
    }
}
