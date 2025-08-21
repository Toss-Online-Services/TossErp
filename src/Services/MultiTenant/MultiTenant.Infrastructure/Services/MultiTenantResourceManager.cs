using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using TossErp.MultiTenant.Domain.Entities;
using TossErp.MultiTenant.Domain.Services;

namespace TossErp.MultiTenant.Infrastructure.Services;

/// <summary>
/// Advanced multi-tenant resource management system with intelligent allocation and monitoring
/// </summary>
public class MultiTenantResourceManager : IMultiTenantResourceManager
{
    private readonly ILogger<MultiTenantResourceManager> _logger;
    private readonly ITenantRepository _tenantRepository;
    private readonly IResourceQuotaRepository _quotaRepository;
    private readonly IResourceUsageRepository _usageRepository;
    private readonly ITenantConfigurationService _configurationService;
    private readonly IResourceProvisioningService _provisioningService;
    private readonly IResourceMonitoringService _monitoringService;
    private readonly IMemoryCache _cache;
    private readonly Timer _usageCollectionTimer;
    private readonly Timer _quotaEnforcementTimer;

    public MultiTenantResourceManager(
        ILogger<MultiTenantResourceManager> logger,
        ITenantRepository tenantRepository,
        IResourceQuotaRepository quotaRepository,
        IResourceUsageRepository usageRepository,
        ITenantConfigurationService configurationService,
        IResourceProvisioningService provisioningService,
        IResourceMonitoringService monitoringService,
        IMemoryCache cache)
    {
        _logger = logger;
        _tenantRepository = tenantRepository;
        _quotaRepository = quotaRepository;
        _usageRepository = usageRepository;
        _configurationService = configurationService;
        _provisioningService = provisioningService;
        _monitoringService = monitoringService;
        _cache = cache;

        // Initialize background timers
        _usageCollectionTimer = new Timer(CollectUsageMetrics, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        _quotaEnforcementTimer = new Timer(EnforceQuotas, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }

    public async Task<TenantResourceAllocation> AllocateResourcesAsync(
        string tenantId,
        ResourceAllocationRequest request,
        CancellationToken cancellationToken = default)
    {
        var allocationId = Guid.NewGuid().ToString();
        _logger.LogInformation("Allocating resources for tenant {TenantId}, allocation {AllocationId}", tenantId, allocationId);

        try
        {
            // Get tenant configuration
            var tenant = await _tenantRepository.GetTenantAsync(tenantId, cancellationToken);
            if (tenant == null)
            {
                throw new InvalidOperationException($"Tenant {tenantId} not found");
            }

            if (tenant.Status != TenantStatus.Active)
            {
                throw new InvalidOperationException($"Tenant {tenantId} is not active (status: {tenant.Status})");
            }

            // Get current quotas and usage
            var quotas = await GetTenantQuotasAsync(tenantId, cancellationToken);
            var currentUsage = await GetCurrentUsageAsync(tenantId, cancellationToken);

            // Validate resource availability
            var validationResult = ValidateResourceRequest(request, quotas, currentUsage);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Resource allocation denied for tenant {TenantId}: {Reason}", tenantId, validationResult.Reason);
                throw new ResourceQuotaExceededException(validationResult.Reason);
            }

            // Calculate optimal resource allocation
            var allocation = await CalculateOptimalAllocationAsync(tenantId, request, quotas, currentUsage, cancellationToken);

            // Provision resources
            var provisioningResult = await _provisioningService.ProvisionResourcesAsync(allocation, cancellationToken);
            if (!provisioningResult.Success)
            {
                _logger.LogError("Failed to provision resources for tenant {TenantId}: {Error}", tenantId, provisioningResult.ErrorMessage);
                throw new ResourceProvisioningException(provisioningResult.ErrorMessage);
            }

            // Update allocation record
            allocation.Id = allocationId;
            allocation.Status = ResourceAllocationStatus.Active;
            allocation.AllocatedAt = DateTime.UtcNow;
            allocation.ProvisioningDetails = provisioningResult.Details;

            // Save allocation
            await _quotaRepository.SaveAllocationAsync(allocation, cancellationToken);

            // Update usage tracking
            await UpdateResourceUsageAsync(tenantId, allocation, ResourceUsageOperation.Allocate, cancellationToken);

            // Cache allocation for quick access
            _cache.Set($"allocation_{allocationId}", allocation, TimeSpan.FromHours(1));

            _logger.LogInformation("Successfully allocated resources for tenant {TenantId}, allocation {AllocationId}", tenantId, allocationId);
            return allocation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to allocate resources for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<bool> DeallocateResourcesAsync(
        string tenantId,
        string allocationId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deallocating resources for tenant {TenantId}, allocation {AllocationId}", tenantId, allocationId);

        try
        {
            // Get allocation
            var allocation = await GetAllocationAsync(allocationId, cancellationToken);
            if (allocation == null)
            {
                _logger.LogWarning("Allocation {AllocationId} not found", allocationId);
                return false;
            }

            if (allocation.TenantId != tenantId)
            {
                throw new UnauthorizedAccessException($"Allocation {allocationId} does not belong to tenant {tenantId}");
            }

            if (allocation.Status != ResourceAllocationStatus.Active)
            {
                _logger.LogWarning("Allocation {AllocationId} is not active (status: {Status})", allocationId, allocation.Status);
                return false;
            }

            // Deprovision resources
            var deprovisioningResult = await _provisioningService.DeprovisionResourcesAsync(allocation, cancellationToken);
            if (!deprovisioningResult.Success)
            {
                _logger.LogError("Failed to deprovision resources for allocation {AllocationId}: {Error}", allocationId, deprovisioningResult.ErrorMessage);
                throw new ResourceProvisioningException(deprovisioningResult.ErrorMessage);
            }

            // Update allocation status
            allocation.Status = ResourceAllocationStatus.Deallocated;
            allocation.DeallocatedAt = DateTime.UtcNow;
            allocation.DeprovisioningDetails = deprovisioningResult.Details;

            // Save allocation
            await _quotaRepository.SaveAllocationAsync(allocation, cancellationToken);

            // Update usage tracking
            await UpdateResourceUsageAsync(tenantId, allocation, ResourceUsageOperation.Deallocate, cancellationToken);

            // Remove from cache
            _cache.Remove($"allocation_{allocationId}");

            _logger.LogInformation("Successfully deallocated resources for tenant {TenantId}, allocation {AllocationId}", tenantId, allocationId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deallocate resources for tenant {TenantId}, allocation {AllocationId}", tenantId, allocationId);
            throw;
        }
    }

    public async Task<TenantResourceUsage> GetTenantUsageAsync(
        string tenantId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting resource usage for tenant {TenantId}", tenantId);

        try
        {
            var cacheKey = $"usage_{tenantId}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";
            if (_cache.TryGetValue(cacheKey, out TenantResourceUsage cachedUsage))
            {
                return cachedUsage;
            }

            var usage = await _usageRepository.GetTenantUsageAsync(tenantId, startDate, endDate, cancellationToken);
            
            // Calculate derived metrics
            usage.CalculateDerivedMetrics();

            // Cache for 5 minutes
            _cache.Set(cacheKey, usage, TimeSpan.FromMinutes(5));

            return usage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get usage for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<List<TenantResourceAllocation>> GetTenantAllocationsAsync(
        string tenantId,
        ResourceAllocationFilter filter,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting resource allocations for tenant {TenantId}", tenantId);

        try
        {
            var allocations = await _quotaRepository.GetTenantAllocationsAsync(tenantId, cancellationToken);

            // Apply filters
            if (filter.ResourceType.HasValue)
            {
                allocations = allocations.Where(a => a.ResourceType == filter.ResourceType.Value).ToList();
            }

            if (filter.Status.HasValue)
            {
                allocations = allocations.Where(a => a.Status == filter.Status.Value).ToList();
            }

            if (filter.AllocatedAfter.HasValue)
            {
                allocations = allocations.Where(a => a.AllocatedAt >= filter.AllocatedAfter.Value).ToList();
            }

            if (filter.AllocatedBefore.HasValue)
            {
                allocations = allocations.Where(a => a.AllocatedAt <= filter.AllocatedBefore.Value).ToList();
            }

            // Apply pagination
            if (filter.Skip.HasValue)
            {
                allocations = allocations.Skip(filter.Skip.Value).ToList();
            }

            if (filter.Take.HasValue)
            {
                allocations = allocations.Take(filter.Take.Value).ToList();
            }

            return allocations.OrderByDescending(a => a.AllocatedAt).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get allocations for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<TenantQuota> UpdateTenantQuotaAsync(
        string tenantId,
        ResourceType resourceType,
        QuotaUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating quota for tenant {TenantId}, resource type {ResourceType}", tenantId, resourceType);

        try
        {
            // Validate tenant exists
            var tenant = await _tenantRepository.GetTenantAsync(tenantId, cancellationToken);
            if (tenant == null)
            {
                throw new InvalidOperationException($"Tenant {tenantId} not found");
            }

            // Get current quota
            var currentQuota = await _quotaRepository.GetTenantQuotaAsync(tenantId, resourceType, cancellationToken);
            if (currentQuota == null)
            {
                // Create new quota
                currentQuota = new TenantQuota
                {
                    TenantId = tenantId,
                    ResourceType = resourceType,
                    CreatedAt = DateTime.UtcNow
                };
            }

            // Validate quota update
            var currentUsage = await GetCurrentUsageAsync(tenantId, cancellationToken);
            var currentResourceUsage = currentUsage.ResourceUsages.FirstOrDefault(r => r.ResourceType == resourceType);
            
            if (currentResourceUsage != null && request.MaxUnits < currentResourceUsage.CurrentUsage)
            {
                throw new InvalidOperationException($"Cannot set quota below current usage. Current usage: {currentResourceUsage.CurrentUsage}, requested quota: {request.MaxUnits}");
            }

            // Update quota properties
            currentQuota.MaxUnits = request.MaxUnits;
            currentQuota.MaxConcurrentUnits = request.MaxConcurrentUnits ?? currentQuota.MaxConcurrentUnits;
            currentQuota.MaxDailyUnits = request.MaxDailyUnits ?? currentQuota.MaxDailyUnits;
            currentQuota.MaxMonthlyUnits = request.MaxMonthlyUnits ?? currentQuota.MaxMonthlyUnits;
            currentQuota.BurstAllowance = request.BurstAllowance ?? currentQuota.BurstAllowance;
            currentQuota.Priority = request.Priority ?? currentQuota.Priority;
            currentQuota.UpdatedAt = DateTime.UtcNow;

            // Save quota
            await _quotaRepository.SaveQuotaAsync(currentQuota, cancellationToken);

            // Clear related caches
            _cache.Remove($"quota_{tenantId}");

            _logger.LogInformation("Successfully updated quota for tenant {TenantId}, resource type {ResourceType}", tenantId, resourceType);
            return currentQuota;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update quota for tenant {TenantId}, resource type {ResourceType}", tenantId, resourceType);
            throw;
        }
    }

    public async Task<ResourceOptimizationSuggestions> GetOptimizationSuggestionsAsync(
        string tenantId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting optimization suggestions for tenant {TenantId}", tenantId);

        try
        {
            var usage = await GetTenantUsageAsync(tenantId, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow, cancellationToken);
            var quotas = await GetTenantQuotasAsync(tenantId, cancellationToken);
            var allocations = await GetTenantAllocationsAsync(tenantId, new ResourceAllocationFilter { Status = ResourceAllocationStatus.Active }, cancellationToken);

            var suggestions = new ResourceOptimizationSuggestions
            {
                TenantId = tenantId,
                GeneratedAt = DateTime.UtcNow,
                Suggestions = new List<OptimizationSuggestion>()
            };

            // Analyze usage patterns and generate suggestions
            foreach (var resourceUsage in usage.ResourceUsages)
            {
                var quota = quotas.FirstOrDefault(q => q.ResourceType == resourceUsage.ResourceType);
                if (quota == null) continue;

                // Check for underutilized resources
                var utilizationRate = (double)resourceUsage.AverageUsage / quota.MaxUnits;
                if (utilizationRate < 0.3 && resourceUsage.AverageUsage > 0)
                {
                    suggestions.Suggestions.Add(new OptimizationSuggestion
                    {
                        Type = OptimizationType.ReduceQuota,
                        ResourceType = resourceUsage.ResourceType,
                        Title = $"Reduce {resourceUsage.ResourceType} quota",
                        Description = $"Current utilization is only {utilizationRate:P0}. Consider reducing quota from {quota.MaxUnits} to {Math.Max(resourceUsage.PeakUsage * 1.2, resourceUsage.AverageUsage * 2)}",
                        EstimatedSavings = CalculateSavings(resourceUsage.ResourceType, quota.MaxUnits - Math.Max(resourceUsage.PeakUsage * 1.2, resourceUsage.AverageUsage * 2)),
                        Impact = OptimizationImpact.Low,
                        Priority = OptimizationPriority.Medium
                    });
                }

                // Check for overutilized resources
                if (utilizationRate > 0.8)
                {
                    suggestions.Suggestions.Add(new OptimizationSuggestion
                    {
                        Type = OptimizationType.IncreaseQuota,
                        ResourceType = resourceUsage.ResourceType,
                        Title = $"Increase {resourceUsage.ResourceType} quota",
                        Description = $"Current utilization is {utilizationRate:P0}. Consider increasing quota from {quota.MaxUnits} to {quota.MaxUnits * 1.5}",
                        EstimatedSavings = 0, // This is a cost increase but prevents performance issues
                        Impact = OptimizationImpact.High,
                        Priority = OptimizationPriority.High
                    });
                }

                // Check for idle allocations
                var idleAllocations = allocations.Where(a => 
                    a.ResourceType == resourceUsage.ResourceType && 
                    a.LastUsedAt.HasValue && 
                    a.LastUsedAt.Value < DateTime.UtcNow.AddDays(-7)).ToList();

                if (idleAllocations.Any())
                {
                    suggestions.Suggestions.Add(new OptimizationSuggestion
                    {
                        Type = OptimizationType.DeallocateIdle,
                        ResourceType = resourceUsage.ResourceType,
                        Title = $"Deallocate idle {resourceUsage.ResourceType} resources",
                        Description = $"Found {idleAllocations.Count} idle allocations not used in the last 7 days",
                        EstimatedSavings = idleAllocations.Sum(a => CalculateSavings(a.ResourceType, a.AllocatedUnits)),
                        Impact = OptimizationImpact.Low,
                        Priority = OptimizationPriority.Medium
                    });
                }
            }

            // Calculate total potential savings
            suggestions.TotalEstimatedSavings = suggestions.Suggestions.Sum(s => s.EstimatedSavings);

            return suggestions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get optimization suggestions for tenant {TenantId}", tenantId);
            throw;
        }
    }

    private async Task<List<TenantQuota>> GetTenantQuotasAsync(string tenantId, CancellationToken cancellationToken)
    {
        var cacheKey = $"quota_{tenantId}";
        if (_cache.TryGetValue(cacheKey, out List<TenantQuota> cachedQuotas))
        {
            return cachedQuotas;
        }

        var quotas = await _quotaRepository.GetTenantQuotasAsync(tenantId, cancellationToken);
        _cache.Set(cacheKey, quotas, TimeSpan.FromMinutes(15));
        return quotas;
    }

    private async Task<TenantResourceUsage> GetCurrentUsageAsync(string tenantId, CancellationToken cancellationToken)
    {
        return await GetTenantUsageAsync(tenantId, DateTime.UtcNow.Date, DateTime.UtcNow, cancellationToken);
    }

    private ResourceValidationResult ValidateResourceRequest(
        ResourceAllocationRequest request,
        List<TenantQuota> quotas,
        TenantResourceUsage currentUsage)
    {
        var quota = quotas.FirstOrDefault(q => q.ResourceType == request.ResourceType);
        if (quota == null)
        {
            return new ResourceValidationResult { IsValid = false, Reason = $"No quota defined for resource type {request.ResourceType}" };
        }

        var resourceUsage = currentUsage.ResourceUsages.FirstOrDefault(r => r.ResourceType == request.ResourceType);
        var currentResourceUsage = resourceUsage?.CurrentUsage ?? 0;

        // Check max units
        if (currentResourceUsage + request.RequestedUnits > quota.MaxUnits)
        {
            return new ResourceValidationResult { IsValid = false, Reason = $"Requested units would exceed quota. Current: {currentResourceUsage}, Requested: {request.RequestedUnits}, Quota: {quota.MaxUnits}" };
        }

        // Check daily limits
        if (quota.MaxDailyUnits.HasValue)
        {
            var dailyUsage = resourceUsage?.DailyUsage ?? 0;
            if (dailyUsage + request.RequestedUnits > quota.MaxDailyUnits.Value)
            {
                return new ResourceValidationResult { IsValid = false, Reason = $"Requested units would exceed daily quota. Daily usage: {dailyUsage}, Requested: {request.RequestedUnits}, Daily quota: {quota.MaxDailyUnits.Value}" };
            }
        }

        // Check monthly limits
        if (quota.MaxMonthlyUnits.HasValue)
        {
            var monthlyUsage = resourceUsage?.MonthlyUsage ?? 0;
            if (monthlyUsage + request.RequestedUnits > quota.MaxMonthlyUnits.Value)
            {
                return new ResourceValidationResult { IsValid = false, Reason = $"Requested units would exceed monthly quota. Monthly usage: {monthlyUsage}, Requested: {request.RequestedUnits}, Monthly quota: {quota.MaxMonthlyUnits.Value}" };
            }
        }

        return new ResourceValidationResult { IsValid = true };
    }

    private async Task<TenantResourceAllocation> CalculateOptimalAllocationAsync(
        string tenantId,
        ResourceAllocationRequest request,
        List<TenantQuota> quotas,
        TenantResourceUsage currentUsage,
        CancellationToken cancellationToken)
    {
        var quota = quotas.First(q => q.ResourceType == request.ResourceType);
        
        // Calculate optimal allocation based on usage patterns and request
        var allocation = new TenantResourceAllocation
        {
            TenantId = tenantId,
            ResourceType = request.ResourceType,
            RequestedUnits = request.RequestedUnits,
            AllocatedUnits = request.RequestedUnits, // Start with requested amount
            Status = ResourceAllocationStatus.Pending,
            Priority = quota.Priority,
            Configuration = request.Configuration ?? new Dictionary<string, object>(),
            Tags = request.Tags ?? new Dictionary<string, string>()
        };

        // Apply intelligent optimization based on tenant's usage patterns
        var resourceUsage = currentUsage.ResourceUsages.FirstOrDefault(r => r.ResourceType == request.ResourceType);
        if (resourceUsage != null)
        {
            // If tenant typically uses less than requested, optimize allocation
            if (resourceUsage.AverageUsage > 0 && request.RequestedUnits > resourceUsage.AverageUsage * 3)
            {
                var optimizedUnits = Math.Max(resourceUsage.PeakUsage * 1.2, request.RequestedUnits * 0.7);
                allocation.AllocatedUnits = (long)Math.Min(optimizedUnits, request.RequestedUnits);
                allocation.Tags["optimization"] = "usage_pattern_based";
            }
        }

        return allocation;
    }

    private async Task UpdateResourceUsageAsync(
        string tenantId,
        TenantResourceAllocation allocation,
        ResourceUsageOperation operation,
        CancellationToken cancellationToken)
    {
        var usageRecord = new ResourceUsageRecord
        {
            TenantId = tenantId,
            ResourceType = allocation.ResourceType,
            Operation = operation,
            Units = allocation.AllocatedUnits,
            Timestamp = DateTime.UtcNow,
            AllocationId = allocation.Id,
            Metadata = new Dictionary<string, object>
            {
                ["priority"] = allocation.Priority.ToString(),
                ["configuration"] = allocation.Configuration
            }
        };

        await _usageRepository.RecordUsageAsync(usageRecord, cancellationToken);

        // Clear usage cache
        _cache.Remove($"usage_{tenantId}");
    }

    private async Task<TenantResourceAllocation?> GetAllocationAsync(string allocationId, CancellationToken cancellationToken)
    {
        // Try cache first
        if (_cache.TryGetValue($"allocation_{allocationId}", out TenantResourceAllocation cachedAllocation))
        {
            return cachedAllocation;
        }

        // Fallback to repository
        return await _quotaRepository.GetAllocationAsync(allocationId, cancellationToken);
    }

    private decimal CalculateSavings(ResourceType resourceType, double units)
    {
        // Simple cost calculation - in production, use actual pricing data
        var costPerUnit = resourceType switch
        {
            ResourceType.Compute => 0.10m,
            ResourceType.Storage => 0.02m,
            ResourceType.Memory => 0.05m,
            ResourceType.Network => 0.01m,
            ResourceType.Database => 0.15m,
            ResourceType.AICredits => 0.001m,
            _ => 0.05m
        };

        return (decimal)units * costPerUnit * 24 * 30; // Monthly savings
    }

    private void CollectUsageMetrics(object? state)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await _monitoringService.CollectAllTenantsUsageAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during usage metrics collection");
            }
        });
    }

    private void EnforceQuotas(object? state)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await _monitoringService.EnforceQuotasAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during quota enforcement");
            }
        });
    }

    public void Dispose()
    {
        _usageCollectionTimer?.Dispose();
        _quotaEnforcementTimer?.Dispose();
    }
}

// Supporting classes and enums
public enum ResourceType
{
    Compute,
    Storage,
    Memory,
    Network,
    Database,
    AICredits,
    Bandwidth,
    APIRequests
}

public enum ResourceAllocationStatus
{
    Pending,
    Active,
    Deallocated,
    Failed
}

public enum ResourceUsageOperation
{
    Allocate,
    Deallocate,
    Usage,
    Adjustment
}

public enum OptimizationType
{
    ReduceQuota,
    IncreaseQuota,
    DeallocateIdle,
    ConsolidateResources,
    UpgradeType,
    ScheduleOptimization
}

public enum OptimizationImpact
{
    Low,
    Medium,
    High,
    Critical
}

public enum OptimizationPriority
{
    Low,
    Medium,
    High,
    Urgent
}

public enum TenantStatus
{
    Active,
    Suspended,
    Inactive,
    PendingActivation
}
