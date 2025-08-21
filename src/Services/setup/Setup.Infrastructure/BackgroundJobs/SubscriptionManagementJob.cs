using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Setup.Application.Common.Interfaces;
using Setup.Domain.Enums;

namespace Setup.Infrastructure.BackgroundJobs;

public class SubscriptionManagementJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SubscriptionManagementJob> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1); // Run every hour

    public SubscriptionManagementJob(IServiceProvider serviceProvider, ILogger<SubscriptionManagementJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Subscription Management Job started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessSubscriptionsAsync(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Subscription Management Job is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Subscription Management Job");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Wait 5 minutes before retrying
            }
        }

        _logger.LogInformation("Subscription Management Job stopped");
    }

    private async Task ProcessSubscriptionsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<ISetupUnitOfWork>();

        try
        {
            _logger.LogDebug("Processing subscription renewals and expirations");

            // Process expiring subscriptions
            await ProcessExpiringSubscriptionsAsync(tenantService, emailService, cancellationToken);

            // Process expired subscriptions
            await ProcessExpiredSubscriptionsAsync(tenantService, emailService, cancellationToken);

            // Process pending subscription renewals
            await ProcessPendingRenewalsAsync(tenantService, emailService, cancellationToken);

            // Process trial conversions
            await ProcessTrialConversionsAsync(tenantService, emailService, cancellationToken);

            // Update subscription metrics
            await UpdateSubscriptionMetricsAsync(unitOfWork, cancellationToken);

            _logger.LogDebug("Subscription processing completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing subscriptions");
        }
    }

    private async Task ProcessExpiringSubscriptionsAsync(ITenantService tenantService, IEmailService emailService, 
        CancellationToken cancellationToken)
    {
        // Get subscriptions expiring in the next 7 days
        var expiringSubscriptions = await tenantService.GetExpiringSubscriptionsAsync(7, cancellationToken);

        foreach (var subscription in expiringSubscriptions)
        {
            try
            {
                _logger.LogInformation("Processing expiring subscription for tenant: {TenantId}", subscription.TenantId);

                // Send expiry notification
                var tenant = await tenantService.GetTenantAsync(subscription.TenantId, cancellationToken);
                if (tenant != null)
                {
                    await emailService.SendSubscriptionExpiryNotificationAsync(
                        tenant.ContactEmail, 
                        tenant.Name, 
                        subscription.EndDate, 
                        cancellationToken);
                }

                // Mark as notified to avoid sending multiple notifications
                await tenantService.MarkSubscriptionAsNotifiedAsync(subscription.TenantId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing expiring subscription for tenant: {TenantId}", 
                    subscription.TenantId);
            }
        }
    }

    private async Task ProcessExpiredSubscriptionsAsync(ITenantService tenantService, IEmailService emailService,
        CancellationToken cancellationToken)
    {
        // Get recently expired subscriptions (within last 24 hours)
        var expiredSubscriptions = await tenantService.GetExpiredSubscriptionsAsync(1, cancellationToken);

        foreach (var subscription in expiredSubscriptions)
        {
            try
            {
                _logger.LogInformation("Processing expired subscription for tenant: {TenantId}", subscription.TenantId);

                var tenant = await tenantService.GetTenantAsync(subscription.TenantId, cancellationToken);
                if (tenant != null)
                {
                    // Suspend tenant if subscription has expired
                    if (tenant.Status == TenantStatus.Active)
                    {
                        await tenantService.SuspendTenantAsync(subscription.TenantId, 
                            "Subscription expired", cancellationToken);

                        _logger.LogInformation("Suspended tenant {TenantId} due to expired subscription", 
                            subscription.TenantId);
                    }

                    // Send suspension notification
                    await emailService.SendEmailAsync(new Setup.Domain.ValueObjects.EmailMessage
                    {
                        To = new[] { tenant.ContactEmail },
                        Subject = "Account Suspended - Subscription Expired",
                        IsHtml = true,
                        Body = GenerateSubscriptionExpiredEmailHtml(tenant.Name, subscription.EndDate)
                    }, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing expired subscription for tenant: {TenantId}", 
                    subscription.TenantId);
            }
        }
    }

    private async Task ProcessPendingRenewalsAsync(ITenantService tenantService, IEmailService emailService,
        CancellationToken cancellationToken)
    {
        var pendingRenewals = await tenantService.GetPendingRenewalsAsync(cancellationToken);

        foreach (var renewal in pendingRenewals)
        {
            try
            {
                _logger.LogInformation("Processing pending renewal for tenant: {TenantId}", renewal.TenantId);

                // Attempt to process the renewal
                var success = await tenantService.ProcessSubscriptionRenewalAsync(renewal.TenantId, cancellationToken);

                if (success)
                {
                    _logger.LogInformation("Successfully renewed subscription for tenant: {TenantId}", renewal.TenantId);

                    var tenant = await tenantService.GetTenantAsync(renewal.TenantId, cancellationToken);
                    if (tenant != null)
                    {
                        await emailService.SendEmailAsync(new Setup.Domain.ValueObjects.EmailMessage
                        {
                            To = new[] { tenant.ContactEmail },
                            Subject = "Subscription Renewed Successfully",
                            IsHtml = true,
                            Body = GenerateSubscriptionRenewedEmailHtml(tenant.Name, renewal.NewEndDate)
                        }, cancellationToken);
                    }
                }
                else
                {
                    _logger.LogWarning("Failed to renew subscription for tenant: {TenantId}", renewal.TenantId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing pending renewal for tenant: {TenantId}", renewal.TenantId);
            }
        }
    }

    private async Task ProcessTrialConversionsAsync(ITenantService tenantService, IEmailService emailService,
        CancellationToken cancellationToken)
    {
        var expiredTrials = await tenantService.GetExpiredTrialsAsync(cancellationToken);

        foreach (var trial in expiredTrials)
        {
            try
            {
                _logger.LogInformation("Processing expired trial for tenant: {TenantId}", trial.TenantId);

                var tenant = await tenantService.GetTenantAsync(trial.TenantId, cancellationToken);
                if (tenant != null)
                {
                    // Check if trial was converted to paid subscription
                    var hasActiveSubscription = await tenantService.HasActiveSubscriptionAsync(trial.TenantId, cancellationToken);

                    if (!hasActiveSubscription)
                    {
                        // Suspend tenant due to expired trial
                        await tenantService.SuspendTenantAsync(trial.TenantId, 
                            "Trial period expired", cancellationToken);

                        await emailService.SendEmailAsync(new Setup.Domain.ValueObjects.EmailMessage
                        {
                            To = new[] { tenant.ContactEmail },
                            Subject = "Trial Period Expired - TOSS ERP",
                            IsHtml = true,
                            Body = GenerateTrialExpiredEmailHtml(tenant.Name)
                        }, cancellationToken);

                        _logger.LogInformation("Suspended tenant {TenantId} due to expired trial", trial.TenantId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing expired trial for tenant: {TenantId}", trial.TenantId);
            }
        }
    }

    private async Task UpdateSubscriptionMetricsAsync(ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Updating subscription metrics");

            var today = DateTime.UtcNow.Date;
            var metrics = new SubscriptionMetrics
            {
                Date = today,
                ActiveSubscriptions = await unitOfWork.TenantRepository.GetActiveSubscriptionCountAsync(cancellationToken),
                ExpiredSubscriptions = await unitOfWork.TenantRepository.GetExpiredSubscriptionCountAsync(cancellationToken),
                TrialSubscriptions = await unitOfWork.TenantRepository.GetTrialSubscriptionCountAsync(cancellationToken),
                SuspendedTenants = await unitOfWork.TenantRepository.GetSuspendedTenantCountAsync(cancellationToken),
                MonthlyRevenue = await unitOfWork.TenantRepository.GetMonthlyRevenueAsync(today.Year, today.Month, cancellationToken),
                ChurnRate = await CalculateChurnRateAsync(unitOfWork, cancellationToken)
            };

            await unitOfWork.SystemConfigRepository.UpsertSubscriptionMetricsAsync(metrics, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Subscription metrics updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating subscription metrics");
        }
    }

    private async Task<decimal> CalculateChurnRateAsync(ISetupUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        var lastMonth = DateTime.UtcNow.AddMonths(-1);
        var startOfLastMonth = new DateTime(lastMonth.Year, lastMonth.Month, 1);
        var endOfLastMonth = startOfLastMonth.AddMonths(1).AddDays(-1);

        var customersAtStartOfMonth = await unitOfWork.TenantRepository.GetActiveCustomerCountAsync(startOfLastMonth, cancellationToken);
        var lostCustomers = await unitOfWork.TenantRepository.GetLostCustomerCountAsync(startOfLastMonth, endOfLastMonth, cancellationToken);

        return customersAtStartOfMonth > 0 ? (decimal)lostCustomers / customersAtStartOfMonth * 100 : 0;
    }

    #region Email Templates

    private static string GenerateSubscriptionExpiredEmailHtml(string tenantName, DateTime expiredDate)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Account Suspended - Subscription Expired</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h1 style='color: #e74c3c;'>Account Suspended</h1>
        <p>Hello,</p>
        <p>We regret to inform you that the TOSS ERP account for <strong>{tenantName}</strong> has been suspended due to subscription expiry.</p>
        <p><strong>Subscription expired on:</strong> {expiredDate:yyyy-MM-dd}</p>
        <p>To restore access to your account, please renew your subscription by clicking the button below:</p>
        <div style='text-align: center; margin: 30px 0;'>
            <a href='#' style='background-color: #27ae60; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>Renew Subscription</a>
        </div>
        <p>If you have any questions or need assistance, please contact our support team.</p>
        <p>Best regards,<br>The TOSS ERP Team</p>
    </div>
</body>
</html>";
    }

    private static string GenerateSubscriptionRenewedEmailHtml(string tenantName, DateTime newEndDate)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Subscription Renewed Successfully</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h1 style='color: #27ae60;'>Subscription Renewed Successfully</h1>
        <p>Hello,</p>
        <p>Great news! The TOSS ERP subscription for <strong>{tenantName}</strong> has been renewed successfully.</p>
        <p><strong>New subscription end date:</strong> {newEndDate:yyyy-MM-dd}</p>
        <p>You can continue using all TOSS ERP features without interruption.</p>
        <p>Thank you for choosing TOSS ERP for your business needs!</p>
        <p>Best regards,<br>The TOSS ERP Team</p>
    </div>
</body>
</html>";
    }

    private static string GenerateTrialExpiredEmailHtml(string tenantName)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Trial Period Expired</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h1 style='color: #f39c12;'>Trial Period Expired</h1>
        <p>Hello,</p>
        <p>The trial period for <strong>{tenantName}</strong> has expired, and your account has been temporarily suspended.</p>
        <p>We hope you've had a chance to explore the powerful features of TOSS ERP during your trial!</p>
        <p>To continue using TOSS ERP and restore full access to your account, please choose a subscription plan:</p>
        <div style='text-align: center; margin: 30px 0;'>
            <a href='#' style='background-color: #3498db; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block; margin: 5px;'>View Plans</a>
            <a href='#' style='background-color: #27ae60; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block; margin: 5px;'>Subscribe Now</a>
        </div>
        <p>If you have any questions about our plans or need assistance, please contact our support team.</p>
        <p>Best regards,<br>The TOSS ERP Team</p>
    </div>
</body>
</html>";
    }

    #endregion
}

// Supporting classes
public class SubscriptionMetrics
{
    public DateTime Date { get; set; }
    public int ActiveSubscriptions { get; set; }
    public int ExpiredSubscriptions { get; set; }
    public int TrialSubscriptions { get; set; }
    public int SuspendedTenants { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal ChurnRate { get; set; }
}
