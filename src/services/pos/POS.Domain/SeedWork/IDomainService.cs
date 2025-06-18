namespace POS.Domain.SeedWork;

/// <summary>
/// Base domain service interface
/// </summary>
public interface IDomainService
{
    Task PublishDomainEventsAsync(CancellationToken cancellationToken = default);
} 
