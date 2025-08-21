using Microsoft.Extensions.Logging;
using Setup.Application.Common.Interfaces;
using Setup.Domain.Aggregates.TenantAggregate;
using Setup.Domain.Enums;
using Setup.Domain.ValueObjects;

namespace Setup.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly ISetupUnitOfWork _unitOfWork;
    private readonly ILogger<TenantService> _logger;

    public TenantService(ISetupUnitOfWork unitOfWork, ILogger<TenantService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Tenant> CreateTenantAsync(string name, string code, string planName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new tenant: {Name} with code: {Code}", name, code);

        // Validate unique code
        var isCodeUnique = await _unitOfWork.TenantRepository.IsCodeUniqueAsync(code, null, cancellationToken);
        if (!isCodeUnique)
        {
            throw new InvalidOperationException($"Tenant code '{code}' is already in use");
        }

        // Create tenant with default values
        var tenant = new Tenant
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Code = code,
            Description = $"Tenant for {name}",
            Status = TenantStatus.Trial,
            DatabaseName = $"TenantDb_{code}",
            ConnectionString = GenerateConnectionString(code),
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            SubscriptionPlan = CreateDefaultSubscriptionPlan(planName),
            BillingCycle = CreateTrialBillingCycle(),
            UsageQuota = CreateDefaultUsageQuota(planName)
        };

        await _unitOfWork.TenantRepository.AddAsync(tenant, cancellationToken);

        // Create default subscription
        var subscription = new Subscription
        {
            Id = Guid.NewGuid().ToString(),
            TenantId = tenant.Id,
            PlanId = planName,
            Status = SubscriptionStatus.Trial,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30), // 30-day trial
            TrialEndDate = DateTime.UtcNow.AddDays(30),
            IsAutoRenew = false
        };

        await _unitOfWork.TenantRepository.AddSubscriptionAsync(subscription, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created tenant: {TenantId}", tenant.Id);
        return tenant;
    }

    public async Task<bool> ActivateTenantAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Activating tenant: {TenantId}", tenantId);

        var tenant = await _unitOfWork.TenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant == null)
        {
            _logger.LogWarning("Tenant not found: {TenantId}", tenantId);
            return false;
        }

        tenant.Status = TenantStatus.Active;
        tenant.IsActive = true;
        
        _unitOfWork.TenantRepository.Update(tenant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully activated tenant: {TenantId}", tenantId);
        return true;
    }

    public async Task<bool> SuspendTenantAsync(string tenantId, string reason, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Suspending tenant: {TenantId} for reason: {Reason}", tenantId, reason);

        var tenant = await _unitOfWork.TenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant == null)
        {
            _logger.LogWarning("Tenant not found: {TenantId}", tenantId);
            return false;
        }

        tenant.Status = TenantStatus.Suspended;
        tenant.IsActive = false;
        
        _unitOfWork.TenantRepository.Update(tenant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully suspended tenant: {TenantId}", tenantId);
        return true;
    }

    public async Task<bool> DeactivateTenantAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deactivating tenant: {TenantId}", tenantId);

        var tenant = await _unitOfWork.TenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant == null)
        {
            _logger.LogWarning("Tenant not found: {TenantId}", tenantId);
            return false;
        }

        tenant.Status = TenantStatus.Inactive;
        tenant.IsActive = false;
        
        _unitOfWork.TenantRepository.Update(tenant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully deactivated tenant: {TenantId}", tenantId);
        return true;
    }

    public async Task<bool> UpdateSubscriptionAsync(string tenantId, string newPlanName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating subscription for tenant: {TenantId} to plan: {PlanName}", tenantId, newPlanName);

        var tenant = await _unitOfWork.TenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant == null)
        {
            _logger.LogWarning("Tenant not found: {TenantId}", tenantId);
            return false;
        }

        // Update subscription plan
        tenant.SubscriptionPlan = CreateSubscriptionPlan(newPlanName);
        tenant.UsageQuota = CreateUsageQuota(newPlanName);
        
        // Update subscription record
        var subscription = await _unitOfWork.TenantRepository.GetSubscriptionByTenantIdAsync(tenantId, cancellationToken);
        if (subscription != null)
        {
            subscription.PlanId = newPlanName;
            subscription.Status = SubscriptionStatus.Active;
            _unitOfWork.TenantRepository.UpdateSubscription(subscription);
        }

        _unitOfWork.TenantRepository.Update(tenant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully updated subscription for tenant: {TenantId}", tenantId);
        return true;
    }

    public async Task<bool> UpdateUsageAsync(string tenantId, long storageUsed, int usersCount, long apiCalls, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating usage for tenant: {TenantId}", tenantId);

        var tenant = await _unitOfWork.TenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant == null)
        {
            _logger.LogWarning("Tenant not found: {TenantId}", tenantId);
            return false;
        }

        // Update usage quota
        var quota = tenant.UsageQuota;
        quota.StorageUsed = storageUsed;
        quota.UsersCount = usersCount;
        quota.ApiCallsThisMonth = apiCalls;
        quota.LastResetDate = DateTime.UtcNow;

        tenant.UsageQuota = quota;
        _unitOfWork.TenantRepository.Update(tenant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Check for quota violations
        await CheckQuotaViolationsAsync(tenant, cancellationToken);

        return true;
    }

    public async Task<IEnumerable<Tenant>> GetExpiringTenantsAsync(int daysBeforeExpiry, CancellationToken cancellationToken = default)
    {
        var beforeDate = DateTime.UtcNow.AddDays(daysBeforeExpiry);
        return await _unitOfWork.TenantRepository.GetExpiringSubscriptionsAsync(beforeDate, cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetOverQuotaTenantsAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.TenantRepository.GetOverQuotaTenantsAsync(cancellationToken);
    }

    public async Task<TenantMetrics> GetTenantMetricsAsync(CancellationToken cancellationToken = default)
    {
        var totalTenants = await _unitOfWork.TenantRepository.GetTotalTenantsCountAsync(cancellationToken);
        var activeTenants = await _unitOfWork.TenantRepository.GetActiveTenantsCountAsync(cancellationToken);
        var totalRevenue = await _unitOfWork.TenantRepository.GetTotalRevenueAsync(cancellationToken);

        var expiringTenants = await GetExpiringTenantsAsync(7, cancellationToken);
        var overQuotaTenants = await GetOverQuotaTenantsAsync(cancellationToken);

        return new TenantMetrics
        {
            TotalTenants = totalTenants,
            ActiveTenants = activeTenants,
            InactiveTenants = totalTenants - activeTenants,
            MonthlyRecurringRevenue = totalRevenue,
            ExpiringWithinWeek = expiringTenants.Count(),
            OverQuota = overQuotaTenants.Count()
        };
    }

    public async Task ProcessExpiringSubscriptionsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing expiring subscriptions");

        var expiringTenants = await GetExpiringTenantsAsync(0, cancellationToken); // Already expired
        
        foreach (var tenant in expiringTenants)
        {
            if (tenant.BillingCycle.IsAutoRenew)
            {
                await RenewSubscriptionAsync(tenant.Id, cancellationToken);
            }
            else
            {
                await SuspendTenantAsync(tenant.Id, "Subscription expired", cancellationToken);
            }
        }

        _logger.LogInformation("Processed {Count} expiring subscriptions", expiringTenants.Count());
    }

    private async Task<bool> RenewSubscriptionAsync(string tenantId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Renewing subscription for tenant: {TenantId}", tenantId);

        var tenant = await _unitOfWork.TenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant == null) return false;

        // Extend billing cycle
        var billingCycle = tenant.BillingCycle;
        billingCycle.StartDate = billingCycle.EndDate;
        billingCycle.EndDate = CalculateNextBillingDate(billingCycle.Type, billingCycle.StartDate);
        
        tenant.BillingCycle = billingCycle;
        _unitOfWork.TenantRepository.Update(tenant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task CheckQuotaViolationsAsync(Tenant tenant, CancellationToken cancellationToken)
    {
        var quota = tenant.UsageQuota;
        var violations = new List<string>();

        if (quota.StorageUsed > quota.StorageLimit)
            violations.Add($"Storage quota exceeded: {quota.StorageUsed:N0} / {quota.StorageLimit:N0} bytes");

        if (quota.UsersCount > quota.UsersLimit)
            violations.Add($"User quota exceeded: {quota.UsersCount} / {quota.UsersLimit} users");

        if (quota.ApiCallsThisMonth > quota.ApiCallsLimit)
            violations.Add($"API quota exceeded: {quota.ApiCallsThisMonth:N0} / {quota.ApiCallsLimit:N0} calls");

        if (violations.Any())
        {
            _logger.LogWarning("Quota violations for tenant {TenantId}: {Violations}", 
                tenant.Id, string.Join(", ", violations));

            // Here you could implement notification logic or automatic actions
        }
    }

    private static SubscriptionPlan CreateDefaultSubscriptionPlan(string planName) =>
        CreateSubscriptionPlan(planName);

    private static SubscriptionPlan CreateSubscriptionPlan(string planName)
    {
        return planName.ToLower() switch
        {
            "starter" => new SubscriptionPlan
            {
                PlanName = "Starter",
                PlanType = PlanType.Basic,
                MaxUsers = 5,
                MaxStorage = 1_000_000_000, // 1GB
                Price = 29.99m,
                Currency = "USD",
                Features = "Basic features, email support"
            },
            "professional" => new SubscriptionPlan
            {
                PlanName = "Professional",
                PlanType = PlanType.Professional,
                MaxUsers = 25,
                MaxStorage = 10_000_000_000, // 10GB
                Price = 99.99m,
                Currency = "USD",
                Features = "Advanced features, priority support, integrations"
            },
            "enterprise" => new SubscriptionPlan
            {
                PlanName = "Enterprise",
                PlanType = PlanType.Enterprise,
                MaxUsers = 500,
                MaxStorage = 100_000_000_000, // 100GB
                Price = 499.99m,
                Currency = "USD",
                Features = "All features, dedicated support, custom integrations"
            },
            _ => new SubscriptionPlan
            {
                PlanName = "Trial",
                PlanType = PlanType.Trial,
                MaxUsers = 3,
                MaxStorage = 500_000_000, // 500MB
                Price = 0m,
                Currency = "USD",
                Features = "Limited trial features"
            }
        };
    }

    private static BillingCycle CreateTrialBillingCycle()
    {
        return new BillingCycle
        {
            Type = BillingCycleType.Trial,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30),
            IsAutoRenew = false,
            GracePeriodDays = 7
        };
    }

    private static UsageQuota CreateDefaultUsageQuota(string planName) =>
        CreateUsageQuota(planName);

    private static UsageQuota CreateUsageQuota(string planName)
    {
        var plan = CreateSubscriptionPlan(planName);
        return new UsageQuota
        {
            StorageUsed = 0,
            StorageLimit = plan.MaxStorage,
            UsersCount = 0,
            UsersLimit = plan.MaxUsers,
            ApiCallsThisMonth = 0,
            ApiCallsLimit = planName.ToLower() switch
            {
                "starter" => 10_000,
                "professional" => 100_000,
                "enterprise" => 1_000_000,
                _ => 1_000
            },
            LastResetDate = DateTime.UtcNow
        };
    }

    private static string GenerateConnectionString(string tenantCode)
    {
        // In a real implementation, this would generate a proper connection string
        // based on your database configuration
        return $"Server=(localdb)\\mssqllocaldb;Database=TossErp_Tenant_{tenantCode};Trusted_Connection=true;MultipleActiveResultSets=true";
    }

    private static DateTime CalculateNextBillingDate(BillingCycleType cycleType, DateTime startDate)
    {
        return cycleType switch
        {
            BillingCycleType.Monthly => startDate.AddMonths(1),
            BillingCycleType.Quarterly => startDate.AddMonths(3),
            BillingCycleType.Yearly => startDate.AddYears(1),
            _ => startDate.AddMonths(1)
        };
    }
}

public class TenantMetrics
{
    public int TotalTenants { get; set; }
    public int ActiveTenants { get; set; }
    public int InactiveTenants { get; set; }
    public decimal MonthlyRecurringRevenue { get; set; }
    public int ExpiringWithinWeek { get; set; }
    public int OverQuota { get; set; }
}
