using TossErp.POS.Domain.AggregatesModel.BuyerAggregate;
using TossErp.POS.Domain.Common;
using TossErp.POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TossErp.POS.Infrastructure.Repositories;

public class BuyerRepository : IBuyerRepository
{
    private readonly POSContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public BuyerRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Buyer?> GetByIdAsync(string id)
    {
        return await _context.Buyers
            .Include(b => b.PaymentMethods)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Buyer>> GetAllAsync()
    {
        return await _context.Buyers
            .Include(b => b.PaymentMethods)
            .ToListAsync();
    }

    public async Task AddAsync(Buyer buyer)
    {
        await _context.Buyers.AddAsync(buyer);
    }

    public void Update(Buyer buyer)
    {
        _context.Entry(buyer).State = EntityState.Modified;
    }

    public void Delete(Buyer buyer)
    {
        _context.Buyers.Remove(buyer);
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Buyers.AnyAsync(b => b.Id == id);
    }

    public async Task<Buyer?> GetByIdentityGuidAsync(string identityGuid)
    {
        return await _context.Buyers
            .Include(b => b.PaymentMethods)
            .FirstOrDefaultAsync(b => b.IdentityGuid == identityGuid);
    }
}
