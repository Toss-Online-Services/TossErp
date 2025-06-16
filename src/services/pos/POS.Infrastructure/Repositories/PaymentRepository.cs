using System.Linq.Expressions;
using POS.Domain.AggregatesModel.PaymentAggregate;
using POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;

namespace TossErp.POS.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly POSContext _context;

    public PaymentRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Payment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Payments.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> FindAsync(Expression<Func<Payment, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task Add(Payment payment, CancellationToken cancellationToken = default)
    {
        await _context.Payments.AddAsync(payment, cancellationToken);
    }

    public async Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        _context.Entry(payment).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Payments.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null)
        {
            _context.Payments.Remove(entity);
        }
    }

    public async Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.FirstOrDefaultAsync(p => p.TransactionId == transactionId, cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByStoreAsync(string storeId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.Where(p => p.StoreId == storeId).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.Where(p => p.Status == status).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetBySaleIdAsync(Guid saleId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.Where(p => p.SaleId == saleId).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByPaymentMethodIdAsync(Guid paymentMethodId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Include(p => p.PaymentMethod)
            .Include(p => p.CardType)
            .Where(p => p.PaymentMethodId == paymentMethodId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalAmountBySaleIdAsync(Guid saleId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Where(p => p.SaleId == saleId)
            .SumAsync(p => p.Amount, cancellationToken);
    }

    public async Task<decimal> GetTotalAmountByPaymentMethodIdAsync(Guid paymentMethodId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Where(p => p.PaymentMethodId == paymentMethodId)
            .SumAsync(p => p.Amount, cancellationToken);
    }
} 
