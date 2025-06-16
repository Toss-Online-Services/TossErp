using System.Linq.Expressions;
using POS.Domain.AggregatesModel.BuyerAggregate;
using POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ICardTypeRepository = POS.Domain.Repositories.ICardTypeRepository;

namespace TossErp.POS.Infrastructure.Repositories;

public class CardTypeRepository : ICardTypeRepository
{
    private readonly POSContext _context;

    public CardTypeRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<CardType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.CardTypes
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<CardType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CardTypes
            .ToListAsync(cancellationToken);
    }

    public async Task<CardType?> FindAsync(Expression<Func<CardType, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.CardTypes
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task AddAsync(CardType entity, CancellationToken cancellationToken = default)
    {
        await _context.CardTypes.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(CardType entity, CancellationToken cancellationToken = default)
    {
        _context.CardTypes.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var cardType = await _context.CardTypes.FindAsync(new object[] { id }, cancellationToken);
        if (cardType != null)
        {
            _context.CardTypes.Remove(cardType);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<CardType> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.CardTypes.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<CardType>> GetByStoreIdAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _context.CardTypes
            .Where(c => c.StoreId == storeId)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }
} 
