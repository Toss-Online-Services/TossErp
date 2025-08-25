using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.Repositories;

namespace Crm.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of Lead repository for development/testing
/// </summary>
public class InMemoryLeadRepository : ILeadRepository
{
    private readonly List<Lead> _leads = new();
    private readonly object _lock = new();

    public Task<Lead?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var lead = _leads.FirstOrDefault(l => l.Id == id);
            return Task.FromResult(lead);
        }
    }

    public Task<IEnumerable<Lead>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_leads.AsEnumerable());
        }
    }

    public Task<IEnumerable<Lead>> GetByStatusAsync(LeadStatus status, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var leads = _leads.Where(l => l.Status == status);
            return Task.FromResult(leads);
        }
    }

    public Task<IEnumerable<Lead>> GetBySourceAsync(LeadSource source, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var leads = _leads.Where(l => l.Source == source);
            return Task.FromResult(leads);
        }
    }

    public Task<IEnumerable<Lead>> GetByAssigneeAsync(string assignedTo, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var leads = _leads.Where(l => l.AssignedTo == assignedTo);
            return Task.FromResult(leads);
        }
    }

    public Task<IEnumerable<Lead>> GetByScoreRangeAsync(int minScore, int maxScore, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var leads = _leads.Where(l => l.Score.Value >= minScore && l.Score.Value <= maxScore);
            return Task.FromResult(leads);
        }
    }

    public Task<IEnumerable<Lead>> GetHotLeadsAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var leads = _leads.Where(l => l.IsHot);
            return Task.FromResult(leads);
        }
    }

    public Task<IEnumerable<Lead>> GetStaleLeadsAsync(int daysThreshold = 30, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var leads = _leads.Where(l => l.IsStale);
            return Task.FromResult(leads);
        }
    }

    public Task<IEnumerable<Lead>> GetByCampaignAsync(string campaignName, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var leads = _leads.Where(l => l.CampaignName == campaignName);
            return Task.FromResult(leads);
        }
    }

    public Task<IEnumerable<Lead>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var term = searchTerm.ToLowerInvariant();
            var leads = _leads.Where(l => 
                l.FirstName.ToLowerInvariant().Contains(term) ||
                l.LastName.ToLowerInvariant().Contains(term) ||
                l.Company.ToLowerInvariant().Contains(term) ||
                l.Email.Value.ToLowerInvariant().Contains(term) ||
                (l.Phone?.Value?.Contains(term) == true));
            return Task.FromResult(leads);
        }
    }

    public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_leads.Count);
        }
    }

    public Task<int> GetCountByStatusAsync(LeadStatus status, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var count = _leads.Count(l => l.Status == status);
            return Task.FromResult(count);
        }
    }

    public Task<Lead> AddAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _leads.Add(lead);
            return Task.FromResult(lead);
        }
    }

    public Task UpdateAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var existingIndex = _leads.FindIndex(l => l.Id == lead.Id);
            if (existingIndex >= 0)
            {
                _leads[existingIndex] = lead;
            }
            return Task.CompletedTask;
        }
    }

    public Task DeleteAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _leads.RemoveAll(l => l.Id == lead.Id);
            return Task.CompletedTask;
        }
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var exists = _leads.Any(l => l.Id == id);
            return Task.FromResult(exists);
        }
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var exists = _leads.Any(l => l.Email.Value.Equals(email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(exists);
        }
    }
}
