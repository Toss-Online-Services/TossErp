using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.ClientRequestAggregate;
using POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;
using POS.Domain.Common;
using POS.Domain.SeedWork;

namespace TossErp.POS.Infrastructure.Repositories;

public class ClientRequestRepository : IClientRequestRepository
{
    private readonly POSContext _context;

    public ClientRequestRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<ClientRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.ClientRequests.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<ClientRequest>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ClientRequests.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ClientRequest>> FindAsync(Expression<Func<ClientRequest, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.ClientRequests.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ClientRequest clientRequest, CancellationToken cancellationToken = default)
    {
        await _context.ClientRequests.AddAsync(clientRequest, cancellationToken);
    }

    public async Task UpdateAsync(ClientRequest clientRequest, CancellationToken cancellationToken = default)
    {
        _context.Entry(clientRequest).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(ClientRequest clientRequest, CancellationToken cancellationToken = default)
    {
        _context.ClientRequests.Remove(clientRequest);
        await Task.CompletedTask;
    }

    public async Task<ClientRequest?> GetByRequestIdAsync(string requestId, CancellationToken cancellationToken = default)
    {
        return await _context.ClientRequests
            .FirstOrDefaultAsync(c => c.RequestId == requestId, cancellationToken);
    }

    public async Task<IEnumerable<ClientRequest>> GetByStoreAsync(string storeId, CancellationToken cancellationToken = default)
    {
        return await _context.ClientRequests
            .Where(c => c.StoreId == storeId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ClientRequest>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await _context.ClientRequests
            .Where(c => c.Status == status)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }
} 
