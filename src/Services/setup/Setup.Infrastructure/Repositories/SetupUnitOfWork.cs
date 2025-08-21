using Setup.Application.Common.Interfaces;
using Setup.Infrastructure.Data;
using Setup.Infrastructure.Repositories;

namespace Setup.Infrastructure.Repositories;

public class SetupUnitOfWork : ISetupUnitOfWork
{
    private readonly SetupDbContext _context;
    
    private ITenantRepository? _tenantRepository;
    private IUserRepository? _userRepository;
    private ISystemConfigRepository? _systemConfigRepository;

    public SetupUnitOfWork(SetupDbContext context)
    {
        _context = context;
    }

    public ITenantRepository TenantRepository => 
        _tenantRepository ??= new TenantRepository(_context);

    public IUserRepository UserRepository => 
        _userRepository ??= new UserRepository(_context);

    public ISystemConfigRepository SystemConfigRepository => 
        _systemConfigRepository ??= new SystemConfigRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_context.Database.CurrentTransaction == null)
        {
            await _context.Database.BeginTransactionAsync(cancellationToken);
        }
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_context.Database.CurrentTransaction != null)
        {
            await _context.Database.CommitTransactionAsync(cancellationToken);
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_context.Database.CurrentTransaction != null)
        {
            await _context.Database.RollbackTransactionAsync(cancellationToken);
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}
