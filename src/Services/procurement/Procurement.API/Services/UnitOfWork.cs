using TossErp.Procurement.Application.Common.Interfaces;

namespace TossErp.Procurement.API.Services;

/// <summary>
/// Mock implementation of IUnitOfWork for MVP
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(ILogger<UnitOfWork> logger)
    {
        _logger = logger;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("UnitOfWork.SaveChangesAsync called");
        return Task.FromResult(1); // Mock: return 1 affected entity
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("UnitOfWork.BeginTransactionAsync called");
        return Task.CompletedTask;
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("UnitOfWork.CommitTransactionAsync called");
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("UnitOfWork.RollbackTransactionAsync called");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _logger.LogInformation("UnitOfWork.Dispose called");
    }
}
