using Microsoft.EntityFrameworkCore;
using Setup.Application.Common.Interfaces;
using Setup.Domain.Aggregates.TenantAggregate;
using Setup.Infrastructure.Data;

namespace Setup.Infrastructure.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly SetupDbContext _context;

    public TenantRepository(SetupDbContext context)
    {
        _context = context;
    }

    public async Task<Tenant?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .Include(t => t.Subscription)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Tenant?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .Include(t => t.Subscription)
            .FirstOrDefaultAsync(t => t.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .Include(t => t.Subscription)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetActiveTenantsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .Include(t => t.Subscription)
            .Where(t => t.IsActive && t.Status == Domain.Enums.TenantStatus.Active)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetByStatusAsync(Domain.Enums.TenantStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .Include(t => t.Subscription)
            .Where(t => t.Status == status)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetExpiringSubscriptionsAsync(DateTime beforeDate, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .Include(t => t.Subscription)
            .Where(t => t.IsActive && 
                       t.BillingCycle.EndDate <= beforeDate &&
                       t.Status == Domain.Enums.TenantStatus.Active)
            .OrderBy(t => t.BillingCycle.EndDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetOverQuotaTenantsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .Include(t => t.Subscription)
            .Where(t => t.IsActive && 
                       (t.UsageQuota.StorageUsed > t.UsageQuota.StorageLimit ||
                        t.UsageQuota.UsersCount > t.UsageQuota.UsersLimit ||
                        t.UsageQuota.ApiCallsThisMonth > t.UsageQuota.ApiCallsLimit))
            .OrderByDescending(t => t.UsageQuota.StorageUsed)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsCodeUniqueAsync(string code, string? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Tenants.Where(t => t.Code == code);
        
        if (!string.IsNullOrEmpty(excludeId))
        {
            query = query.Where(t => t.Id != excludeId);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<int> GetTotalTenantsCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tenants.CountAsync(cancellationToken);
    }

    public async Task<int> GetActiveTenantsCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .CountAsync(t => t.IsActive && t.Status == Domain.Enums.TenantStatus.Active, cancellationToken);
    }

    public async Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .Where(t => t.IsActive && t.Status == Domain.Enums.TenantStatus.Active)
            .SumAsync(t => t.SubscriptionPlan.Price, cancellationToken);
    }

    public async Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        await _context.Tenants.AddAsync(tenant, cancellationToken);
    }

    public void Update(Tenant tenant)
    {
        _context.Tenants.Update(tenant);
    }

    public void Remove(Tenant tenant)
    {
        _context.Tenants.Remove(tenant);
    }

    // Advanced tenant management methods
    public async Task<IEnumerable<Tenant>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .Include(t => t.Subscription)
            .Where(t => t.Name.Contains(searchTerm) || 
                       t.Code.Contains(searchTerm) || 
                       t.Description!.Contains(searchTerm))
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetBySubscriptionPlanAsync(string planName, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .Include(t => t.Subscription)
            .Where(t => t.SubscriptionPlan.PlanName == planName)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetRecentlyCreatedAsync(int days, CancellationToken cancellationToken = default)
    {
        var fromDate = DateTime.UtcNow.AddDays(-days);
        return await _context.Tenants
            .Include(t => t.Subscription)
            .Where(t => t.CreatedAt >= fromDate)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    // Subscription management methods
    public async Task<Subscription?> GetSubscriptionByTenantIdAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.TenantId == tenantId, cancellationToken);
    }

    public async Task AddSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken = default)
    {
        await _context.Subscriptions.AddAsync(subscription, cancellationToken);
    }

    public void UpdateSubscription(Subscription subscription)
    {
        _context.Subscriptions.Update(subscription);
    }

    public void RemoveSubscription(Subscription subscription)
    {
        _context.Subscriptions.Remove(subscription);
    }
}
