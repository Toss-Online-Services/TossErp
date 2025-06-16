using eShop.POS.Infrastructure.Idempotency;
using POS.Domain.AggregatesModel.SyncAggregate;

namespace TossErp.POS.Infrastructure.Idempotency;

public class RequestManager : IRequestManager
{
    private readonly POSContext _context;

    public RequestManager(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<bool> ExistAsync(string id)
    {
        return await _context.Set<ClientRequest>()
            .AnyAsync(r => r.Id == id);
    }

    public async Task CreateRequestForCommandAsync<T>(string id)
    {
        var exists = await ExistAsync(id);

        var request = exists ?
            throw new InvalidOperationException($"Request with {id} already exists") :
            new ClientRequest()
            {
                Id = id,
                Name = typeof(T).Name,
                Time = DateTime.UtcNow
            };

        _context.Add(request);

        await _context.SaveChangesAsync();
    }
}
