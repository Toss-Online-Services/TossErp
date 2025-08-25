using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.Repositories;
using TossErp.CRM.Domain.ValueObjects;
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
            _logger.LogError(ex, "Error retrieving leads with status: {Status}", status);
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
            _logger.LogError(ex, "Error retrieving leads with source: {Source}", source);
            throw;
        }
    }

    public async Task<IEnumerable<Lead>> GetByAssigneeAsync(string assignedTo, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Leads
                .Where(l => l.AssignedTo == assignedTo && !l.IsDeleted)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leads assigned to: {AssignedTo}", assignedTo);
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
                .ThenByDescending(l => l.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leads with score range: {MinScore}-{MaxScore}", minScore, maxScore);
            throw;
        }
    }

    public async Task<IEnumerable<Lead>> GetHotLeadsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Leads
                .Where(l => l.Score.Value >= 70 && l.Status != LeadStatus.Converted && l.Status != LeadStatus.Disqualified && !l.IsDeleted)
                .OrderByDescending(l => l.Score.Value)
                .ThenByDescending(l => l.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hot leads");
            throw;
        }
    }

    public async Task<IEnumerable<Lead>> GetStaleLeadsAsync(int daysThreshold = 30, CancellationToken cancellationToken = default)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysThreshold);
            return await _context.Leads
                .Where(l => (l.LastContactedAt == null || l.LastContactedAt < cutoffDate) 
                           && l.Status != LeadStatus.Converted 
                           && l.Status != LeadStatus.Disqualified 
                           && !l.IsDeleted)
                .OrderBy(l => l.LastContactedAt ?? l.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving stale leads with {Days} days threshold", daysThreshold);
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
            _logger.LogError(ex, "Error retrieving leads for campaign: {CampaignName}", campaignName);
            throw;
        }
    }

    public async Task<IEnumerable<Lead>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            var lowerSearchTerm = searchTerm.ToLower();
            return await _context.Leads
                .Where(l => !l.IsDeleted && (
                    l.FirstName.ToLower().Contains(lowerSearchTerm) ||
                    l.LastName.ToLower().Contains(lowerSearchTerm) ||
                    l.Email.Value.ToLower().Contains(lowerSearchTerm) ||
                    l.Company.ToLower().Contains(lowerSearchTerm) ||
                    (l.Phone != null && l.Phone.Value.Contains(searchTerm))
                ))
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching leads with term: {SearchTerm}", searchTerm);
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
            _logger.LogError(ex, "Error getting leads count");
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
            _logger.LogError(ex, "Error getting leads count for status: {Status}", status);
            throw;
        }
    }

    public async Task<Lead> AddAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Leads.AddAsync(lead, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return lead;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding lead with ID: {LeadId}", lead.Id);
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
            _logger.LogError(ex, "Error updating lead with ID: {LeadId}", lead.Id);
            throw;
        }
    }

    public async Task DeleteAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        try
        {
            lead.Delete("System"); // Soft delete
            _context.Leads.Update(lead);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting lead with ID: {LeadId}", lead.Id);
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
            _logger.LogError(ex, "Error checking if lead exists with ID: {LeadId}", id);
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
            _logger.LogError(ex, "Error checking if lead exists with email: {Email}", email);
            throw;
        }
    }
}
