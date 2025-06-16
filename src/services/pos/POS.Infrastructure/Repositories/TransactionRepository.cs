using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.TransactionAggregate;
using POS.Domain.Repositories;
using POS.Domain.Common;
using TossErp.POS.Infrastructure.Data;

namespace TossErp.POS.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly POSContext _context;

    public TransactionRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Transaction?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> FindAsync(Expression<Func<Transaction, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Transaction entity, CancellationToken cancellationToken = default)
    {
        await _context.Transactions.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(Transaction entity, CancellationToken cancellationToken = default)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var transaction = await _context.Transactions.FindAsync(new object[] { id }, cancellationToken);
        if (transaction != null)
        {
            _context.Transactions.Remove(transaction);
        }
    }

    public async Task<Transaction?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .FirstOrDefaultAsync(t => t.Reference == reference, cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetByStoreAsync(string storeId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Where(t => t.StoreId == storeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Where(t => t.Status == status)
            .ToListAsync(cancellationToken);
    }
} 
