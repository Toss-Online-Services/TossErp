using System.Linq.Expressions;
using POS.Domain.AggregatesModel.BuyerAggregate;
using POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;

namespace TossErp.POS.Infrastructure.Repositories;

public class BuyerRepository : IBuyerRepository
{
    private readonly POSContext _context;

    public BuyerRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Buyer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Buyers.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Buyer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Buyers.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Buyer>> FindAsync(Expression<Func<Buyer, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Buyers.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Buyer> AddAsync(Buyer buyer, CancellationToken cancellationToken = default)
    {
        await _context.Buyers.AddAsync(buyer, cancellationToken);
        return buyer;
    }

    public void Update(Buyer buyer)
    {
        _context.Entry(buyer).State = EntityState.Modified;
    }

    public void Delete(Buyer buyer)
    {
        _context.Buyers.Remove(buyer);
    }

    public async Task<Buyer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Buyers.FirstOrDefaultAsync(b => b.Email == email, cancellationToken);
    }

    public async Task<Buyer> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        return await _context.Buyers.FirstOrDefaultAsync(b => b.Phone == phone, cancellationToken);
    }

    public async Task<IEnumerable<Buyer>> GetByStoreIdAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _context.Buyers
            .Include(b => b.PaymentMethods)
            .Where(b => b.StoreId == storeId)
            .OrderBy(b => b.Name)
            .ToListAsync(cancellationToken);
    }
}
