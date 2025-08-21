using Microsoft.Extensions.Logging;
using Setup.Application.Common.Interfaces;
using Setup.Domain.ValueObjects;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Setup.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ISendGridClient? _sendGridClient;
    private readonly ILogger<EmailService> _logger;
    private readonly ISetupUnitOfWork _unitOfWork;

    public EmailService(
        ISendGridClient? sendGridClient,
        ILogger<EmailService> logger,
        ISetupUnitOfWork unitOfWork)
    {
        _sendGridClient = sendGridClient;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_sendGridClient == null)
            {
                _logger.LogWarning("SendGrid client not configured. Email will not be sent.");
                return false;
            }

            _logger.LogInformation("Sending email to: {To}, Subject: {Subject}", 
                string.Join(", ", message.To), message.Subject);

            var from = new EmailAddress(message.From ?? "noreply@tosserp.com", message.FromName ?? "TOSS ERP");
            var msg = new SendGridMessage();
            
            msg.SetFrom(from);
            msg.SetSubject(message.Subject);

            // Add recipients
            foreach (var to in message.To)
            {
                msg.AddTo(new EmailAddress(to));
            }

            // Add CC recipients
            if (message.Cc?.Any() == true)
            {
                foreach (var cc in message.Cc)
                {
                    msg.AddCc(new EmailAddress(cc));
                }
            }

            // Add BCC recipients
            if (message.Bcc?.Any() == true)
            {
                foreach (var bcc in message.Bcc)
                {
                    msg.AddBcc(new EmailAddress(bcc));
                }
            }

            // Set content
            if (message.IsHtml)
            {
                msg.SetHtmlContent(message.Body);
                if (!string.IsNullOrEmpty(message.PlainTextBody))
                {
                    msg.SetPlainTextContent(message.PlainTextBody);
                }
            }
            else
            {
                msg.SetPlainTextContent(message.Body);
            }

            // Add attachments
            if (message.Attachments?.Any() == true)
            {
                foreach (var attachment in message.Attachments)
                {
                    msg.AddAttachment(attachment.Filename, attachment.Content, attachment.Type, attachment.Disposition);
                }
            }

            // Set reply-to
            if (!string.IsNullOrEmpty(message.ReplyTo))
            {
                msg.SetReplyTo(new EmailAddress(message.ReplyTo));
            }

            // Add custom headers
            if (message.Headers?.Any() == true)
            {
                foreach (var header in message.Headers)
                {
                    msg.AddHeader(header.Key, header.Value);
                }
            }

            // Set tracking settings
            msg.SetClickTracking(true, true);
            msg.SetOpenTracking(true);
            msg.SetSubscriptionTracking(false);

            // Send the email
            var response = await _sendGridClient.SendEmailAsync(msg, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email sent successfully to: {To}", string.Join(", ", message.To));
                await LogEmailSentAsync(message, true, null, cancellationToken);
                return true;
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogError("Failed to send email. Status: {Status}, Response: {Response}", 
                    response.StatusCode, responseBody);
                await LogEmailSentAsync(message, false, responseBody, cancellationToken);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to: {To}", string.Join(", ", message.To));
            await LogEmailSentAsync(message, false, ex.Message, cancellationToken);
            return false;
        }
    }

    public async Task<bool> SendBulkEmailAsync(IEnumerable<EmailMessage> messages, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_sendGridClient == null)
            {
                _logger.LogWarning("SendGrid client not configured. Bulk emails will not be sent.");
                return false;
            }

            _logger.LogInformation("Sending bulk email to {Count} recipients", messages.Count());

            var tasks = messages.Select(message => SendEmailAsync(message, cancellationToken));
            var results = await Task.WhenAll(tasks);

            var successCount = results.Count(r => r);
            var totalCount = results.Length;

            _logger.LogInformation("Bulk email completed: {Success}/{Total} emails sent successfully", 
                successCount, totalCount);

            return successCount == totalCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending bulk emails");
            return false;
        }
    }

    public async Task<bool> SendTemplatedEmailAsync(string templateId, object templateData, 
        IEnumerable<string> recipients, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_sendGridClient == null)
            {
                _logger.LogWarning("SendGrid client not configured. Templated email will not be sent.");
                return false;
            }

            _logger.LogInformation("Sending templated email using template: {TemplateId} to {Count} recipients", 
                templateId, recipients.Count());

            var from = new EmailAddress("noreply@tosserp.com", "TOSS ERP");
            var msg = new SendGridMessage();
            
            msg.SetFrom(from);
            msg.SetTemplateId(templateId);

            // Add recipients as personalizations
            foreach (var recipient in recipients)
            {
                var personalization = new Personalization();
                personalization.AddTo(new EmailAddress(recipient));
                personalization.SetTemplateData(templateData);
                msg.AddPersonalization(personalization);
            }

            var response = await _sendGridClient.SendEmailAsync(msg, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Templated email sent successfully to {Count} recipients", recipients.Count());
                
                // Log each recipient
                foreach (var recipient in recipients)
                {
                    await LogTemplatedEmailSentAsync(templateId, recipient, true, null, cancellationToken);
                }
                return true;
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogError("Failed to send templated email. Status: {Status}, Response: {Response}", 
                    response.StatusCode, responseBody);
                
                foreach (var recipient in recipients)
                {
                    await LogTemplatedEmailSentAsync(templateId, recipient, false, responseBody, cancellationToken);
                }
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending templated email");
            foreach (var recipient in recipients)
            {
                await LogTemplatedEmailSentAsync(templateId, recipient, false, ex.Message, cancellationToken);
            }
            return false;
        }
    }

    public async Task<bool> SendWelcomeEmailAsync(string userEmail, string userName, string activationLink,
        CancellationToken cancellationToken = default)
    {
        var message = new EmailMessage
        {
            To = new[] { userEmail },
            Subject = "Welcome to TOSS ERP!",
            IsHtml = true,
            Body = GenerateWelcomeEmailHtml(userName, activationLink),
            PlainTextBody = GenerateWelcomeEmailText(userName, activationLink)
        };

        return await SendEmailAsync(message, cancellationToken);
    }

    public async Task<bool> SendPasswordResetEmailAsync(string userEmail, string userName, string resetLink,
        CancellationToken cancellationToken = default)
    {
        var message = new EmailMessage
        {
            To = new[] { userEmail },
            Subject = "Password Reset Request - TOSS ERP",
            IsHtml = true,
            Body = GeneratePasswordResetEmailHtml(userName, resetLink),
            PlainTextBody = GeneratePasswordResetEmailText(userName, resetLink)
        };

        return await SendEmailAsync(message, cancellationToken);
    }

    public async Task<bool> SendAccountLockedEmailAsync(string userEmail, string userName, DateTime lockoutEnd,
        CancellationToken cancellationToken = default)
    {
        var message = new EmailMessage
        {
            To = new[] { userEmail },
            Subject = "Account Temporarily Locked - TOSS ERP",
            IsHtml = true,
            Body = GenerateAccountLockedEmailHtml(userName, lockoutEnd),
            PlainTextBody = GenerateAccountLockedEmailText(userName, lockoutEnd)
        };

        return await SendEmailAsync(message, cancellationToken);
    }

    public async Task<bool> SendSubscriptionExpiryNotificationAsync(string userEmail, string tenantName, 
        DateTime expiryDate, CancellationToken cancellationToken = default)
    {
        var message = new EmailMessage
        {
            To = new[] { userEmail },
            Subject = "Subscription Expiry Notification - TOSS ERP",
            IsHtml = true,
            Body = GenerateSubscriptionExpiryEmailHtml(tenantName, expiryDate),
            PlainTextBody = GenerateSubscriptionExpiryEmailText(tenantName, expiryDate)
        };

        return await SendEmailAsync(message, cancellationToken);
    }

    public async Task<bool> SendUsageQuotaWarningAsync(string userEmail, string tenantName, string resourceType,
        decimal usagePercentage, CancellationToken cancellationToken = default)
    {
        var message = new EmailMessage
        {
            To = new[] { userEmail },
            Subject = "Usage Quota Warning - TOSS ERP",
            IsHtml = true,
            Body = GenerateUsageQuotaWarningEmailHtml(tenantName, resourceType, usagePercentage),
            PlainTextBody = GenerateUsageQuotaWarningEmailText(tenantName, resourceType, usagePercentage)
        };

        return await SendEmailAsync(message, cancellationToken);
    }

    public async Task<EmailStatistics> GetEmailStatisticsAsync(DateTime? fromDate = null, DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        fromDate ??= DateTime.UtcNow.AddDays(-30);
        toDate ??= DateTime.UtcNow;

        var stats = await _unitOfWork.SystemConfigRepository.GetEmailStatisticsAsync(fromDate.Value, toDate.Value, cancellationToken);
        
        return new EmailStatistics
        {
            TotalEmailsSent = stats.TotalSent,
            TotalEmailsDelivered = stats.TotalDelivered,
            TotalEmailsFailed = stats.TotalFailed,
            WelcomeEmailsSent = stats.WelcomeEmailsSent,
            PasswordResetEmailsSent = stats.PasswordResetEmailsSent,
            NotificationEmailsSent = stats.NotificationEmailsSent,
            DeliveryRate = stats.TotalSent > 0 ? (decimal)stats.TotalDelivered / stats.TotalSent * 100 : 0,
            FailureRate = stats.TotalSent > 0 ? (decimal)stats.TotalFailed / stats.TotalSent * 100 : 0,
            PeriodStart = fromDate.Value,
            PeriodEnd = toDate.Value
        };
    }

    private async Task LogEmailSentAsync(EmailMessage message, bool success, string? errorMessage,
        CancellationToken cancellationToken)
    {
        try
        {
            var log = new EmailLog
            {
                Id = Guid.NewGuid().ToString(),
                To = string.Join(", ", message.To),
                Subject = message.Subject,
                Success = success,
                ErrorMessage = errorMessage,
                SentAt = DateTime.UtcNow,
                TenantId = GetCurrentTenantId(),
                UserId = GetCurrentUserId()
            };

            await _unitOfWork.SystemConfigRepository.AddEmailLogAsync(log, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging email");
        }
    }

    private async Task LogTemplatedEmailSentAsync(string templateId, string recipient, bool success, 
        string? errorMessage, CancellationToken cancellationToken)
    {
        try
        {
            var log = new EmailLog
            {
                Id = Guid.NewGuid().ToString(),
                To = recipient,
                Subject = $"Template: {templateId}",
                Success = success,
                ErrorMessage = errorMessage,
                SentAt = DateTime.UtcNow,
                TenantId = GetCurrentTenantId(),
                UserId = GetCurrentUserId()
            };

            await _unitOfWork.SystemConfigRepository.AddEmailLogAsync(log, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging templated email");
        }
    }

    private static string GetCurrentTenantId()
    {
        // TODO: Get from current context
        return "system";
    }

    private static string GetCurrentUserId()
    {
        // TODO: Get from current context
        return "system";
    }

    #region Email Template Generators

    private static string GenerateWelcomeEmailHtml(string userName, string activationLink)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Welcome to TOSS ERP</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h1 style='color: #2c3e50;'>Welcome to TOSS ERP!</h1>
        <p>Hello {userName},</p>
        <p>Welcome to TOSS ERP - your comprehensive business management solution. We're excited to have you on board!</p>
        <p>To get started, please activate your account by clicking the button below:</p>
        <div style='text-align: center; margin: 30px 0;'>
            <a href='{activationLink}' style='background-color: #3498db; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>Activate Account</a>
        </div>
        <p>If the button doesn't work, you can also copy and paste the following link into your browser:</p>
        <p style='word-break: break-all;'>{activationLink}</p>
        <p>Best regards,<br>The TOSS ERP Team</p>
    </div>
</body>
</html>";
    }

    private static string GenerateWelcomeEmailText(string userName, string activationLink)
    {
        return $@"Welcome to TOSS ERP!

Hello {userName},

Welcome to TOSS ERP - your comprehensive business management solution. We're excited to have you on board!

To get started, please activate your account by visiting the following link:
{activationLink}

Best regards,
The TOSS ERP Team";
    }

    private static string GeneratePasswordResetEmailHtml(string userName, string resetLink)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Password Reset Request</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h1 style='color: #e74c3c;'>Password Reset Request</h1>
        <p>Hello {userName},</p>
        <p>We received a request to reset your password for your TOSS ERP account.</p>
        <p>To reset your password, please click the button below:</p>
        <div style='text-align: center; margin: 30px 0;'>
            <a href='{resetLink}' style='background-color: #e74c3c; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>Reset Password</a>
        </div>
        <p>If the button doesn't work, you can also copy and paste the following link into your browser:</p>
        <p style='word-break: break-all;'>{resetLink}</p>
        <p>If you didn't request this password reset, please ignore this email. Your password will remain unchanged.</p>
        <p>This link will expire in 24 hours for security reasons.</p>
        <p>Best regards,<br>The TOSS ERP Team</p>
    </div>
</body>
</html>";
    }

    private static string GeneratePasswordResetEmailText(string userName, string resetLink)
    {
        return $@"Password Reset Request

Hello {userName},

We received a request to reset your password for your TOSS ERP account.

To reset your password, please visit the following link:
{resetLink}

If you didn't request this password reset, please ignore this email. Your password will remain unchanged.

This link will expire in 24 hours for security reasons.

Best regards,
The TOSS ERP Team";
    }

    private static string GenerateAccountLockedEmailHtml(string userName, DateTime lockoutEnd)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Account Temporarily Locked</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h1 style='color: #f39c12;'>Account Temporarily Locked</h1>
        <p>Hello {userName},</p>
        <p>Your TOSS ERP account has been temporarily locked due to multiple failed login attempts.</p>
        <p><strong>Lockout will end:</strong> {lockoutEnd:yyyy-MM-dd HH:mm:ss} UTC</p>
        <p>For security reasons, please wait until the lockout period expires before attempting to log in again.</p>
        <p>If you believe this was not you, please contact our support team immediately.</p>
        <p>Best regards,<br>The TOSS ERP Team</p>
    </div>
</body>
</html>";
    }

    private static string GenerateAccountLockedEmailText(string userName, DateTime lockoutEnd)
    {
        return $@"Account Temporarily Locked

Hello {userName},

Your TOSS ERP account has been temporarily locked due to multiple failed login attempts.

Lockout will end: {lockoutEnd:yyyy-MM-dd HH:mm:ss} UTC

For security reasons, please wait until the lockout period expires before attempting to log in again.

If you believe this was not you, please contact our support team immediately.

Best regards,
The TOSS ERP Team";
    }

    private static string GenerateSubscriptionExpiryEmailHtml(string tenantName, DateTime expiryDate)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Subscription Expiry Notification</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h1 style='color: #f39c12;'>Subscription Expiry Notification</h1>
        <p>Hello,</p>
        <p>This is a reminder that the TOSS ERP subscription for <strong>{tenantName}</strong> will expire soon.</p>
        <p><strong>Expiry Date:</strong> {expiryDate:yyyy-MM-dd}</p>
        <p>To avoid service interruption, please renew your subscription before the expiry date.</p>
        <div style='text-align: center; margin: 30px 0;'>
            <a href='#' style='background-color: #27ae60; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>Renew Subscription</a>
        </div>
        <p>If you have any questions, please contact our support team.</p>
        <p>Best regards,<br>The TOSS ERP Team</p>
    </div>
</body>
</html>";
    }

    private static string GenerateSubscriptionExpiryEmailText(string tenantName, DateTime expiryDate)
    {
        return $@"Subscription Expiry Notification

Hello,

This is a reminder that the TOSS ERP subscription for {tenantName} will expire soon.

Expiry Date: {expiryDate:yyyy-MM-dd}

To avoid service interruption, please renew your subscription before the expiry date.

If you have any questions, please contact our support team.

Best regards,
The TOSS ERP Team";
    }

    private static string GenerateUsageQuotaWarningEmailHtml(string tenantName, string resourceType, decimal usagePercentage)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Usage Quota Warning</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h1 style='color: #e67e22;'>Usage Quota Warning</h1>
        <p>Hello,</p>
        <p>This is a notification that <strong>{tenantName}</strong> is approaching the usage limit for <strong>{resourceType}</strong>.</p>
        <p><strong>Current Usage:</strong> {usagePercentage:F1}% of quota</p>
        <p>Please consider upgrading your plan or reducing usage to avoid service limitations.</p>
        <div style='text-align: center; margin: 30px 0;'>
            <a href='#' style='background-color: #3498db; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>Upgrade Plan</a>
        </div>
        <p>If you have any questions, please contact our support team.</p>
        <p>Best regards,<br>The TOSS ERP Team</p>
    </div>
</body>
</html>";
    }

    private static string GenerateUsageQuotaWarningEmailText(string tenantName, string resourceType, decimal usagePercentage)
    {
        return $@"Usage Quota Warning

Hello,

This is a notification that {tenantName} is approaching the usage limit for {resourceType}.

Current Usage: {usagePercentage:F1}% of quota

Please consider upgrading your plan or reducing usage to avoid service limitations.

If you have any questions, please contact our support team.

Best regards,
The TOSS ERP Team";
    }

    #endregion
}

public class EmailStatistics
{
    public int TotalEmailsSent { get; set; }
    public int TotalEmailsDelivered { get; set; }
    public int TotalEmailsFailed { get; set; }
    public int WelcomeEmailsSent { get; set; }
    public int PasswordResetEmailsSent { get; set; }
    public int NotificationEmailsSent { get; set; }
    public decimal DeliveryRate { get; set; }
    public decimal FailureRate { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

public class EmailLog
{
    public string Id { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime SentAt { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
