using TossErp.Shared.SeedWork;
using Notifications.Domain.Entities;

namespace Notifications.Domain.Repositories;

public interface INotificationTemplateRepository
{
    Task<NotificationTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<NotificationTemplate?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<NotificationTemplate?> GetByTypeAndChannelAsync(NotificationType type, NotificationChannel channel, string language, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationTemplate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationTemplate>> GetByTypeAsync(NotificationType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationTemplate>> GetByChannelAsync(NotificationChannel channel, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationTemplate>> GetByLanguageAsync(string language, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationTemplate>> GetActiveTemplatesAsync(CancellationToken cancellationToken = default);
    Task AddAsync(NotificationTemplate template, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotificationTemplate template, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
}
