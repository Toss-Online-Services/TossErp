using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.SyncAggregate;
using POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;

namespace TossErp.POS.Infrastructure.Repositories;

public class ClientRequestRepository : IClientRequestRepository
{
    private readonly POSContext _context;

    public ClientRequestRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ClientRequest> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
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

    public async Task<ClientRequest> AddAsync(ClientRequest request, CancellationToken cancellationToken = default)
    {
        await _context.ClientRequests.AddAsync(request, cancellationToken);
        return request;
    }

    public void Update(ClientRequest clientRequest)
    {
        _context.Entry(clientRequest).State = EntityState.Modified;
    }

    public void Delete(ClientRequest clientRequest)
    {
        _context.ClientRequests.Remove(clientRequest);
    }

    public async Task<IEnumerable<ClientRequest>> GetByStoreIdAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _context.ClientRequests
            .Where(c => c.StoreId == storeId)
            .OrderByDescending(c => c.RequestDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ClientRequest>> GetByRequestTypeAsync(string requestType, CancellationToken cancellationToken = default)
    {
        return await _context.ClientRequests.Where(r => r.RequestType == requestType).ToListAsync(cancellationToken);
    }

    public async Task<ClientRequest?> GetLastRequestByStoreIdAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _context.ClientRequests
            .Where(c => c.StoreId == storeId)
            .OrderByDescending(c => c.RequestDate)
            .FirstOrDefaultAsync(cancellationToken);
    }
} 
