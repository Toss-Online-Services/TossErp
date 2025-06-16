using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.BuyerAggregate;
using POS.Domain.Repositories;
using POS.Domain.SeedWork;
using TossErp.POS.Infrastructure.Data;

namespace TossErp.POS.Infrastructure.Repositories;

public class BuyerRepository : IBuyerRepository
{
    private readonly POSContext _context;

    public BuyerRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Buyer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Buyers
            .Include(b => b.PaymentMethods)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Buyer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Buyers
            .Include(b => b.PaymentMethods)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Buyer>> FindAsync(Expression<Func<Buyer, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Buyers
            .Include(b => b.PaymentMethods)
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Buyer buyer, CancellationToken cancellationToken = default)
    {
        await _context.Buyers.AddAsync(buyer, cancellationToken);
    }

    public async Task UpdateAsync(Buyer buyer, CancellationToken cancellationToken = default)
    {
        _context.Entry(buyer).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Buyer buyer, CancellationToken cancellationToken = default)
    {
        _context.Buyers.Remove(buyer);
        await Task.CompletedTask;
    }

    public async Task<Buyer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Buyers
            .Include(b => b.PaymentMethods)
            .FirstOrDefaultAsync(b => b.Email == email, cancellationToken);
    }

    public async Task<Buyer?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        return await _context.Buyers
            .Include(b => b.PaymentMethods)
            .FirstOrDefaultAsync(b => b.Phone == phone, cancellationToken);
    }

    public async Task<IEnumerable<Buyer>> GetByStoreAsync(string storeId, CancellationToken cancellationToken = default)
    {
        return await _context.Buyers
            .Include(b => b.PaymentMethods)
            .Where(b => b.StoreId.ToString() == storeId)
            .OrderBy(b => b.Name)
            .ToListAsync(cancellationToken);
    }
}
