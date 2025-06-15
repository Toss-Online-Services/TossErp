using System.Linq.Expressions;
using POS.Domain.AggregatesModel.SaleAggregate;
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

    public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
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

    public async Task<Payment> AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await _context.Payments.AddAsync(payment, cancellationToken);
        return payment;
    }

    public void Update(Payment payment)
    {
        _context.Entry(payment).State = EntityState.Modified;
    }

    public void Delete(Payment payment)
    {
        _context.Payments.Remove(payment);
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
