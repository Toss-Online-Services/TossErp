using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Setup.Application.Common.Interfaces;

namespace Setup.Infrastructure.BackgroundJobs;

public class UsageQuotaMonitoringJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UsageQuotaMonitoringJob> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(15); // Run every 15 minutes

    public UsageQuotaMonitoringJob(IServiceProvider serviceProvider, ILogger<UsageQuotaMonitoringJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Usage Quota Monitoring Job started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await MonitorUsageQuotasAsync(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Usage Quota Monitoring Job is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Usage Quota Monitoring Job");
                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken); // Wait 2 minutes before retrying
            }
        }

        _logger.LogInformation("Usage Quota Monitoring Job stopped");
    }

    private async Task MonitorUsageQuotasAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<ISetupUnitOfWork>();

        try
        {
            _logger.LogDebug("Monitoring usage quotas for all tenants");

            var activeTenants = await tenantService.GetActiveTenantsAsync(cancellationToken);

            foreach (var tenant in activeTenants)
            {
                await MonitorTenantQuotasAsync(tenant.Id, tenantService, emailService, unitOfWork, cancellationToken);
            }

            _logger.LogDebug("Usage quota monitoring completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error monitoring usage quotas");
        }
    }

    private async Task MonitorTenantQuotasAsync(string tenantId, ITenantService tenantService, 
        IEmailService emailService, ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Monitoring quotas for tenant: {TenantId}", tenantId);

            var tenant = await tenantService.GetTenantAsync(tenantId, cancellationToken);
            if (tenant == null) return;

            var quotaStatus = await tenantService.GetTenantQuotaStatusAsync(tenantId, cancellationToken);

            // Check storage quota
            await CheckStorageQuotaAsync(tenant, quotaStatus.StorageUsage, quotaStatus.StorageQuota, 
                emailService, unitOfWork, cancellationToken);

            // Check user quota
            await CheckUserQuotaAsync(tenant, quotaStatus.UserCount, quotaStatus.UserQuota, 
                emailService, unitOfWork, cancellationToken);

            // Check API call quota
            await CheckApiCallQuotaAsync(tenant, quotaStatus.ApiCallsUsage, quotaStatus.ApiCallQuota, 
                emailService, unitOfWork, cancellationToken);

            // Check transaction quota
            await CheckTransactionQuotaAsync(tenant, quotaStatus.TransactionCount, quotaStatus.TransactionQuota, 
                emailService, unitOfWork, cancellationToken);

            // Check document quota
            await CheckDocumentQuotaAsync(tenant, quotaStatus.DocumentCount, quotaStatus.DocumentQuota, 
                emailService, unitOfWork, cancellationToken);

            // Update usage metrics
            await UpdateUsageMetricsAsync(tenantId, quotaStatus, unitOfWork, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error monitoring quotas for tenant: {TenantId}", tenantId);
        }
    }

    private async Task CheckStorageQuotaAsync(Domain.Aggregates.TenantAggregate.Tenant tenant, long usageBytes, long quotaBytes,
        IEmailService emailService, ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        var usagePercentage = quotaBytes > 0 ? (decimal)usageBytes / quotaBytes * 100 : 0;

        if (usagePercentage >= 95)
        {
            await HandleQuotaViolationAsync(tenant, "Storage", usagePercentage, emailService, unitOfWork, cancellationToken);
        }
        else if (usagePercentage >= 80)
        {
            await HandleQuotaWarningAsync(tenant, "Storage", usagePercentage, emailService, unitOfWork, cancellationToken);
        }
    }

    private async Task CheckUserQuotaAsync(Domain.Aggregates.TenantAggregate.Tenant tenant, int userCount, int userQuota,
        IEmailService emailService, ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        var usagePercentage = userQuota > 0 ? (decimal)userCount / userQuota * 100 : 0;

        if (usagePercentage >= 100)
        {
            await HandleQuotaViolationAsync(tenant, "Users", usagePercentage, emailService, unitOfWork, cancellationToken);
        }
        else if (usagePercentage >= 90)
        {
            await HandleQuotaWarningAsync(tenant, "Users", usagePercentage, emailService, unitOfWork, cancellationToken);
        }
    }

    private async Task CheckApiCallQuotaAsync(Domain.Aggregates.TenantAggregate.Tenant tenant, int apiCallsUsage, int apiCallQuota,
        IEmailService emailService, ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        var usagePercentage = apiCallQuota > 0 ? (decimal)apiCallsUsage / apiCallQuota * 100 : 0;

        if (usagePercentage >= 95)
        {
            await HandleQuotaViolationAsync(tenant, "API Calls", usagePercentage, emailService, unitOfWork, cancellationToken);
        }
        else if (usagePercentage >= 80)
        {
            await HandleQuotaWarningAsync(tenant, "API Calls", usagePercentage, emailService, unitOfWork, cancellationToken);
        }
    }

    private async Task CheckTransactionQuotaAsync(Domain.Aggregates.TenantAggregate.Tenant tenant, int transactionCount, int transactionQuota,
        IEmailService emailService, ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        var usagePercentage = transactionQuota > 0 ? (decimal)transactionCount / transactionQuota * 100 : 0;

        if (usagePercentage >= 95)
        {
            await HandleQuotaViolationAsync(tenant, "Transactions", usagePercentage, emailService, unitOfWork, cancellationToken);
        }
        else if (usagePercentage >= 80)
        {
            await HandleQuotaWarningAsync(tenant, "Transactions", usagePercentage, emailService, unitOfWork, cancellationToken);
        }
    }

    private async Task CheckDocumentQuotaAsync(Domain.Aggregates.TenantAggregate.Tenant tenant, int documentCount, int documentQuota,
        IEmailService emailService, ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        var usagePercentage = documentQuota > 0 ? (decimal)documentCount / documentQuota * 100 : 0;

        if (usagePercentage >= 95)
        {
            await HandleQuotaViolationAsync(tenant, "Documents", usagePercentage, emailService, unitOfWork, cancellationToken);
        }
        else if (usagePercentage >= 80)
        {
            await HandleQuotaWarningAsync(tenant, "Documents", usagePercentage, emailService, unitOfWork, cancellationToken);
        }
    }

    private async Task HandleQuotaWarningAsync(Domain.Aggregates.TenantAggregate.Tenant tenant, string resourceType, 
        decimal usagePercentage, IEmailService emailService, ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        // Check if we've already sent a warning recently (within last 24 hours)
        var recentWarning = await unitOfWork.SystemConfigRepository.GetRecentQuotaWarningAsync(
            tenant.Id, resourceType, DateTime.UtcNow.AddHours(-24), cancellationToken);

        if (recentWarning != null) return; // Already notified recently

        _logger.LogWarning("Quota warning for tenant {TenantId}: {ResourceType} usage at {Usage:F1}%", 
            tenant.Id, resourceType, usagePercentage);

        // Send warning email
        await emailService.SendUsageQuotaWarningAsync(tenant.ContactEmail, tenant.Name, resourceType, usagePercentage, cancellationToken);

        // Record the warning
        var warning = new QuotaWarning
        {
            Id = Guid.NewGuid().ToString(),
            TenantId = tenant.Id,
            ResourceType = resourceType,
            UsagePercentage = usagePercentage,
            WarningType = "WARNING",
            CreatedAt = DateTime.UtcNow,
            IsResolved = false
        };

        await unitOfWork.SystemConfigRepository.AddQuotaWarningAsync(warning, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleQuotaViolationAsync(Domain.Aggregates.TenantAggregate.Tenant tenant, string resourceType, 
        decimal usagePercentage, IEmailService emailService, ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        _logger.LogError("Quota violation for tenant {TenantId}: {ResourceType} usage at {Usage:F1}%", 
            tenant.Id, resourceType, usagePercentage);

        // Send violation email
        await emailService.SendEmailAsync(new Setup.Domain.ValueObjects.EmailMessage
        {
            To = new[] { tenant.ContactEmail },
            Subject = $"URGENT: {resourceType} Quota Exceeded - TOSS ERP",
            IsHtml = true,
            Body = GenerateQuotaViolationEmailHtml(tenant.Name, resourceType, usagePercentage)
        }, cancellationToken);

        // Record the violation
        var violation = new QuotaWarning
        {
            Id = Guid.NewGuid().ToString(),
            TenantId = tenant.Id,
            ResourceType = resourceType,
            UsagePercentage = usagePercentage,
            WarningType = "VIOLATION",
            CreatedAt = DateTime.UtcNow,
            IsResolved = false
        };

        await unitOfWork.SystemConfigRepository.AddQuotaWarningAsync(violation, cancellationToken);

        // For critical resources, consider throttling or temporary suspension
        if (resourceType == "Storage" && usagePercentage >= 100)
        {
            await ThrottleTenantAsync(tenant.Id, resourceType, unitOfWork, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ThrottleTenantAsync(string tenantId, string resourceType, ISetupUnitOfWork unitOfWork, 
        CancellationToken cancellationToken)
    {
        _logger.LogWarning("Throttling tenant {TenantId} due to {ResourceType} quota violation", tenantId, resourceType);

        var throttle = new TenantThrottle
        {
            Id = Guid.NewGuid().ToString(),
            TenantId = tenantId,
            ResourceType = resourceType,
            ThrottleType = "QUOTA_VIOLATION",
            IsActive = true,
            StartedAt = DateTime.UtcNow,
            Reason = $"{resourceType} quota exceeded"
        };

        await unitOfWork.SystemConfigRepository.AddTenantThrottleAsync(throttle, cancellationToken);
    }

    private async Task UpdateUsageMetricsAsync(string tenantId, TenantQuotaStatus quotaStatus, 
        ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        try
        {
            var metrics = new TenantUsageMetrics
            {
                TenantId = tenantId,
                Date = DateTime.UtcNow.Date,
                Hour = DateTime.UtcNow.Hour,
                StorageUsageBytes = quotaStatus.StorageUsage,
                UserCount = quotaStatus.UserCount,
                ApiCallsCount = quotaStatus.ApiCallsUsage,
                TransactionCount = quotaStatus.TransactionCount,
                DocumentCount = quotaStatus.DocumentCount,
                RecordedAt = DateTime.UtcNow
            };

            await unitOfWork.SystemConfigRepository.UpsertTenantUsageMetricsAsync(metrics, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating usage metrics for tenant: {TenantId}", tenantId);
        }
    }

    private static string GenerateQuotaViolationEmailHtml(string tenantName, string resourceType, decimal usagePercentage)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>URGENT: {resourceType} Quota Exceeded</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h1 style='color: #e74c3c;'>🚨 URGENT: Quota Exceeded</h1>
        <p>Hello,</p>
        <p>We need to inform you that <strong>{tenantName}</strong> has exceeded the <strong>{resourceType}</strong> quota limit.</p>
        <div style='background-color: #f8f9fa; padding: 15px; border-left: 4px solid #e74c3c; margin: 20px 0;'>
            <p style='margin: 0;'><strong>Current Usage:</strong> {usagePercentage:F1}% of quota</p>
            <p style='margin: 5px 0 0 0;'><strong>Resource:</strong> {resourceType}</p>
        </div>
        <p><strong>Immediate Action Required:</strong></p>
        <ul>
            <li>Consider upgrading your plan to increase quota limits</li>
            <li>Review and optimize your current {resourceType.ToLowerInvariant()} usage</li>
            <li>Contact our support team for assistance</li>
        </ul>
        <p>Service limitations may apply until quota usage is reduced or your plan is upgraded.</p>
        <div style='text-align: center; margin: 30px 0;'>
            <a href='#' style='background-color: #e74c3c; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block; margin: 5px;'>Upgrade Plan</a>
            <a href='#' style='background-color: #3498db; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block; margin: 5px;'>Contact Support</a>
        </div>
        <p>If you have any questions, please contact our support team immediately.</p>
        <p>Best regards,<br>The TOSS ERP Team</p>
    </div>
</body>
</html>";
    }
}

// Supporting classes
public class TenantQuotaStatus
{
    public long StorageUsage { get; set; }
    public long StorageQuota { get; set; }
    public int UserCount { get; set; }
    public int UserQuota { get; set; }
    public int ApiCallsUsage { get; set; }
    public int ApiCallQuota { get; set; }
    public int TransactionCount { get; set; }
    public int TransactionQuota { get; set; }
    public int DocumentCount { get; set; }
    public int DocumentQuota { get; set; }
}

public class QuotaWarning
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public decimal UsagePercentage { get; set; }
    public string WarningType { get; set; } = string.Empty; // WARNING, VIOLATION
    public DateTime CreatedAt { get; set; }
    public bool IsResolved { get; set; }
}

public class TenantThrottle
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public string ThrottleType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class TenantUsageMetrics
{
    public string TenantId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Hour { get; set; }
    public long StorageUsageBytes { get; set; }
    public int UserCount { get; set; }
    public int ApiCallsCount { get; set; }
    public int TransactionCount { get; set; }
    public int DocumentCount { get; set; }
    public DateTime RecordedAt { get; set; }
}
