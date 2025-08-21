using Microsoft.EntityFrameworkCore.Storage;
using TossErp.Projects.Infrastructure.Data;

namespace TossErp.Projects.Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation for Projects service
/// </summary>
public interface IProjectsUnitOfWork : IDisposable
{
    IProjectRepository Projects { get; }
    IProjectTaskRepository ProjectTasks { get; }
    ITimeEntryRepository TimeEntries { get; }
    IResourceRepository Resources { get; }
    IMilestoneRepository Milestones { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Unit of Work implementation for Projects service
/// </summary>
public class ProjectsUnitOfWork : IProjectsUnitOfWork
{
    private readonly ProjectsDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    private IProjectRepository? _projects;
    private IProjectTaskRepository? _projectTasks;
    private ITimeEntryRepository? _timeEntries;
    private IResourceRepository? _resources;
    private IMilestoneRepository? _milestones;

    public ProjectsUnitOfWork(ProjectsDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IProjectRepository Projects => 
        _projects ??= new ProjectRepository(_context);

    public IProjectTaskRepository ProjectTasks => 
        _projectTasks ??= new ProjectTaskRepository(_context);

    public ITimeEntryRepository TimeEntries => 
        _timeEntries ??= new TimeEntryRepository(_context);

    public IResourceRepository Resources => 
        _resources ??= new ResourceRepository(_context);

    public IMilestoneRepository Milestones => 
        _milestones ??= new MilestoneRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress");
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No transaction in progress");
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No transaction in progress");
        }

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context?.Dispose();
    }
}
