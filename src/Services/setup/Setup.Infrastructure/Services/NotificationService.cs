using Microsoft.Extensions.Logging;
using Setup.Application.Common.Interfaces;
using Setup.Domain.ValueObjects;
using System.Net.Mail;
using System.Text;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Setup.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly ISetupUnitOfWork _unitOfWork;
    private readonly ILogger<NotificationService> _logger;
    private readonly ISendGridClient? _sendGridClient;

    public NotificationService(
        ISetupUnitOfWork unitOfWork, 
        ILogger<NotificationService> logger,
        ISendGridClient? sendGridClient = null)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _sendGridClient = sendGridClient;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = false,
        string? templateId = null, object? templateData = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Sending email to: {To}, Subject: {Subject}", to, subject);

            if (_sendGridClient == null)
            {
                _logger.LogWarning("SendGrid client not configured. Email sending disabled.");
                return false;
            }

            var from = new EmailAddress("noreply@tosserp.com", "TOSS ERP");
            var toEmail = new EmailAddress(to);

            var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, 
                isHtml ? null : body, 
                isHtml ? body : null);

            if (!string.IsNullOrEmpty(templateId) && templateData != null)
            {
                msg.SetTemplateId(templateId);
                msg.SetTemplateData(templateData);
            }

            var response = await _sendGridClient.SendEmailAsync(msg, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email sent successfully to: {To}", to);
                
                // Record email metrics
                await RecordNotificationAsync("email", to, subject, true, cancellationToken);
                return true;
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogError("Failed to send email to: {To}. Status: {Status}, Response: {Response}", 
                    to, response.StatusCode, responseBody);
                
                await RecordNotificationAsync("email", to, subject, false, cancellationToken);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to: {To}", to);
            await RecordNotificationAsync("email", to, subject, false, cancellationToken);
            return false;
        }
    }

    public async Task<bool> SendBulkEmailAsync(IEnumerable<string> recipients, string subject, string body,
        bool isHtml = false, string? templateId = null, object? templateData = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Sending bulk email to {Count} recipients", recipients.Count());

            if (_sendGridClient == null)
            {
                _logger.LogWarning("SendGrid client not configured. Bulk email sending disabled.");
                return false;
            }

            var from = new EmailAddress("noreply@tosserp.com", "TOSS ERP");
            var tos = recipients.Select(email => new EmailAddress(email)).ToList();

            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject,
                isHtml ? null : body,
                isHtml ? body : null);

            if (!string.IsNullOrEmpty(templateId) && templateData != null)
            {
                msg.SetTemplateId(templateId);
                msg.SetTemplateData(templateData);
            }

            var response = await _sendGridClient.SendEmailAsync(msg, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Bulk email sent successfully to {Count} recipients", recipients.Count());
                
                // Record bulk email metrics
                foreach (var recipient in recipients)
                {
                    await RecordNotificationAsync("email", recipient, subject, true, cancellationToken);
                }
                return true;
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogError("Failed to send bulk email. Status: {Status}, Response: {Response}", 
                    response.StatusCode, responseBody);
                
                foreach (var recipient in recipients)
                {
                    await RecordNotificationAsync("email", recipient, subject, false, cancellationToken);
                }
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending bulk email");
            foreach (var recipient in recipients)
            {
                await RecordNotificationAsync("email", recipient, subject, false, cancellationToken);
            }
            return false;
        }
    }

    public async Task<bool> SendTemplatedEmailAsync(string to, string templateId, object templateData,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Sending templated email to: {To}, Template: {TemplateId}", to, templateId);

            var template = await _unitOfWork.SystemConfigRepository.GetNotificationTemplateAsync(templateId, cancellationToken);
            if (template == null)
            {
                _logger.LogWarning("Notification template not found: {TemplateId}", templateId);
                return false;
            }

            if (!template.IsEnabled)
            {
                _logger.LogWarning("Notification template is disabled: {TemplateId}", templateId);
                return false;
            }

            // Replace template variables
            var subject = ReplaceTemplateVariables(template.Subject, templateData);
            var body = ReplaceTemplateVariables(template.Body, templateData);

            var success = await SendEmailAsync(to, subject, body, template.IsHtml, cancellationToken: cancellationToken);

            if (success)
            {
                // Update template usage count
                template.UsageCount++;
                template.LastUsedAt = DateTime.UtcNow;
                _unitOfWork.SystemConfigRepository.UpdateNotificationTemplate(template);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending templated email to: {To}", to);
            return false;
        }
    }

    public async Task<bool> SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("SMS sending not implemented yet. Would send to: {PhoneNumber}", phoneNumber);
            
            // TODO: Implement SMS service (Twilio, etc.)
            await RecordNotificationAsync("sms", phoneNumber, message, false, cancellationToken);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS to: {PhoneNumber}", phoneNumber);
            await RecordNotificationAsync("sms", phoneNumber, message, false, cancellationToken);
            return false;
        }
    }

    public async Task<bool> SendPushNotificationAsync(string userId, string title, string body, 
        Dictionary<string, object>? data = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Push notification sending not implemented yet. Would send to user: {UserId}", userId);
            
            // TODO: Implement push notification service (Firebase, etc.)
            await RecordNotificationAsync("push", userId, title, false, cancellationToken);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending push notification to user: {UserId}", userId);
            await RecordNotificationAsync("push", userId, title, false, cancellationToken);
            return false;
        }
    }

    public async Task<bool> SendInAppNotificationAsync(string userId, string title, string message, 
        string? actionUrl = null, DateTime? expiresAt = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating in-app notification for user: {UserId}", userId);

            var notification = new InAppNotification
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Title = title,
                Message = message,
                ActionUrl = actionUrl,
                IsRead = false,
                Priority = NotificationPriority.Normal,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt ?? DateTime.UtcNow.AddDays(30),
                NotificationType = NotificationType.Information,
                Metadata = new Dictionary<string, object>()
            };

            await _unitOfWork.SystemConfigRepository.AddInAppNotificationAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await RecordNotificationAsync("in-app", userId, title, true, cancellationToken);

            _logger.LogInformation("Successfully created in-app notification for user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending in-app notification to user: {UserId}", userId);
            await RecordNotificationAsync("in-app", userId, title, false, cancellationToken);
            return false;
        }
    }

    public async Task<IEnumerable<InAppNotification>> GetUserNotificationsAsync(string userId, bool unreadOnly = false,
        int limit = 50, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.SystemConfigRepository.GetUserNotificationsAsync(userId, unreadOnly, limit, cancellationToken);
    }

    public async Task<bool> MarkNotificationAsReadAsync(string notificationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = await _unitOfWork.SystemConfigRepository.GetInAppNotificationAsync(notificationId, cancellationToken);
            if (notification == null)
            {
                _logger.LogWarning("In-app notification not found: {NotificationId}", notificationId);
                return false;
            }

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            _unitOfWork.SystemConfigRepository.UpdateInAppNotification(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as read: {NotificationId}", notificationId);
            return false;
        }
    }

    public async Task<bool> MarkAllNotificationsAsReadAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var notifications = await GetUserNotificationsAsync(userId, unreadOnly: true, limit: 1000, cancellationToken);
            
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                _unitOfWork.SystemConfigRepository.UpdateInAppNotification(notification);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Marked all notifications as read for user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read for user: {UserId}", userId);
            return false;
        }
    }

    public async Task<NotificationMetrics> GetNotificationMetricsAsync(DateTime? fromDate = null, DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        fromDate ??= DateTime.UtcNow.AddDays(-30);
        toDate ??= DateTime.UtcNow;

        var metrics = await _unitOfWork.SystemConfigRepository.GetNotificationMetricsAsync(fromDate.Value, toDate.Value, cancellationToken);
        
        return new NotificationMetrics
        {
            TotalNotificationsSent = metrics.TotalSent,
            TotalNotificationsDelivered = metrics.TotalDelivered,
            TotalNotificationsFailed = metrics.TotalFailed,
            EmailNotificationsSent = metrics.EmailSent,
            SmsNotificationsSent = metrics.SmsSent,
            PushNotificationsSent = metrics.PushSent,
            InAppNotificationsSent = metrics.InAppSent,
            DeliveryRate = metrics.TotalSent > 0 ? (decimal)metrics.TotalDelivered / metrics.TotalSent * 100 : 0,
            FailureRate = metrics.TotalSent > 0 ? (decimal)metrics.TotalFailed / metrics.TotalSent * 100 : 0,
            PeriodStart = fromDate.Value,
            PeriodEnd = toDate.Value
        };
    }

    public async Task CleanupExpiredNotificationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Cleaning up expired in-app notifications");

            var expiredNotifications = await _unitOfWork.SystemConfigRepository.GetExpiredNotificationsAsync(cancellationToken);
            
            foreach (var notification in expiredNotifications)
            {
                _unitOfWork.SystemConfigRepository.RemoveInAppNotification(notification);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Cleaned up {Count} expired notifications", expiredNotifications.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired notifications");
        }
    }

    private async Task RecordNotificationAsync(string type, string recipient, string subject, bool success,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var record = new NotificationRecord
            {
                Id = Guid.NewGuid().ToString(),
                Type = type,
                Recipient = recipient,
                Subject = subject,
                Success = success,
                SentAt = DateTime.UtcNow,
                TenantId = "system" // TODO: Get from current context
            };

            await _unitOfWork.SystemConfigRepository.AddNotificationRecordAsync(record, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording notification");
        }
    }

    private static string ReplaceTemplateVariables(string template, object data)
    {
        if (data == null) return template;

        var result = template;
        var properties = data.GetType().GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(data)?.ToString() ?? "";
            result = result.Replace($"{{{property.Name}}}", value);
        }

        return result;
    }
}

public class NotificationMetrics
{
    public int TotalNotificationsSent { get; set; }
    public int TotalNotificationsDelivered { get; set; }
    public int TotalNotificationsFailed { get; set; }
    public int EmailNotificationsSent { get; set; }
    public int SmsNotificationsSent { get; set; }
    public int PushNotificationsSent { get; set; }
    public int InAppNotificationsSent { get; set; }
    public decimal DeliveryRate { get; set; }
    public decimal FailureRate { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

public class NotificationRecord
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public bool Success { get; set; }
    public DateTime SentAt { get; set; }
    public string TenantId { get; set; } = string.Empty;
}
