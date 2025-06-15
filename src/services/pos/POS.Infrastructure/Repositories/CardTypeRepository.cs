using System.Linq.Expressions;
using POS.Domain.AggregatesModel.BuyerAggregate;
using POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;

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
        return await _context.CardTypes.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<CardType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CardTypes.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CardType>> FindAsync(Expression<Func<CardType, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.CardTypes.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<CardType> AddAsync(CardType cardType, CancellationToken cancellationToken = default)
    {
        await _context.CardTypes.AddAsync(cardType, cancellationToken);
        return cardType;
    }

    public void Update(CardType cardType)
    {
        _context.Entry(cardType).State = EntityState.Modified;
    }

    public void Delete(CardType cardType)
    {
        _context.CardTypes.Remove(cardType);
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
