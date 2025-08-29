using TossErp.Shared.SeedWork;
using Notifications.Domain.Entities;

namespace Notifications.Domain.Repositories;

public interface INotificationAuditLogRepository
{
    Task<NotificationAuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationAuditLog>> GetByNotificationIdAsync(Guid notificationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationAuditLog>> GetByRecipientIdAsync(string recipientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationAuditLog>> GetByActionAsync(AuditAction action, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationAuditLog>> GetConsentLogsAsync(string recipientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationAuditLog>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationAuditLog>> GetByChannelAsync(NotificationChannel channel, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationAuditLog>> GetByTypeAsync(NotificationType type, CancellationToken cancellationToken = default);
    Task AddAsync(NotificationAuditLog auditLog, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotificationAuditLog auditLog, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
