namespace TossErp.Assets.Application.Common.Interfaces;

/// <summary>
/// Repository interface for Asset aggregates
/// </summary>
public interface IAssetRepository
{
    Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Asset?> GetByAssetNumberAsync(string assetNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Asset>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Asset>> GetByStatusAsync(AssetStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Asset>> GetByLocationAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Asset>> GetByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Asset>> GetMaintenanceDueAsync(DateTime dueDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Asset>> GetExpiringSoonAsync(DateTime expiryDate, CancellationToken cancellationToken = default);
    Task AddAsync(Asset asset, CancellationToken cancellationToken = default);
    Task UpdateAsync(Asset asset, CancellationToken cancellationToken = default);
    Task DeleteAsync(Asset asset, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Location aggregates
/// </summary>
public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Location>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Location>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Location location, CancellationToken cancellationToken = default);
    Task UpdateAsync(Location location, CancellationToken cancellationToken = default);
    Task DeleteAsync(Location location, CancellationToken cancellationToken = default);
}

/// <summary>
/// Unit of work interface for managing transactions
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Domain event service interface
/// </summary>
public interface IDomainEventService
{
    Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default) where T : class;
    Task PublishAsync(IEnumerable<object> domainEvents, CancellationToken cancellationToken = default);
}

/// <summary>
/// Current user service interface
/// </summary>
public interface ICurrentUserService
{
    string? UserId { get; }
    string? TenantId { get; }
    bool IsAuthenticated { get; }
}

/// <summary>
/// DateTime service interface for testability
/// </summary>
public interface IDateTimeService
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    DateOnly Today { get; }
}

/// <summary>
/// File service interface for asset documents
/// </summary>
public interface IFileService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<Stream> DownloadFileAsync(string fileUrl, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
}

/// <summary>
/// Notification service interface
/// </summary>
public interface INotificationService
{
    Task SendMaintenanceReminderAsync(Guid assetId, string assetName, DateTime maintenanceDate, CancellationToken cancellationToken = default);
    Task SendWarrantyExpiryNotificationAsync(Guid assetId, string assetName, DateTime expiryDate, CancellationToken cancellationToken = default);
    Task SendAssetStatusChangeNotificationAsync(Guid assetId, string assetName, AssetStatus oldStatus, AssetStatus newStatus, CancellationToken cancellationToken = default);
}
