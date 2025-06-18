#nullable enable

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.PaymentAggregate;
using POS.Domain.SeedWork;
using POS.Infrastructure.EntityConfigurations;

namespace POS.Infrastructure.Repositories;

public class PaymentRepository : IRepository<Payment>
{
    private readonly POSContext _context;

    public PaymentRepository(POSContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Payment> AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Payments.AddAsync(payment, cancellationToken);
        return entry.Entity;
    }

    public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetAsync(Specification<Payment> specification, CancellationToken cancellationToken = default)
    {
        var query = _context.Payments.AsQueryable();
        if (specification != null)
        {
            var predicate = specification.ToExpression();
            query = query.Where(predicate);
        }
        return await query.OrderByDescending(p => p.PaymentDate).ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        _context.Entry(payment).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        _context.Payments.Remove(payment);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var payment = await _context.Payments.FindAsync(new object[] { id }, cancellationToken);
        if (payment != null)
        {
            _context.Payments.Remove(payment);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(Specification<Payment> specification, CancellationToken cancellationToken = default)
    {
        var query = _context.Payments.AsQueryable();
        if (specification != null)
        {
            var predicate = specification.ToExpression();
            query = query.Where(predicate);
        }
        return await query.CountAsync(cancellationToken);
    }

    // Overloads without CancellationToken
    public Task<Payment> AddAsync(Payment payment) => AddAsync(payment, default);
    public Task UpdateAsync(Payment payment) => UpdateAsync(payment, default);
    public Task DeleteAsync(Payment payment) => DeleteAsync(payment, default);

    public async Task<Payment> GetAsync(int paymentId)
    {
        var payment = await _context.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Id == paymentId);

        return payment;
    }

    public async Task<Payment> GetByTransactionIdAsync(string transactionId)
    {
        var payment = await _context.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.TransactionId == transactionId);

        return payment;
    }

    public async Task<IEnumerable<Payment>> GetByOrderAsync(int orderId)
    {
        return await _context.Payments
            .Where(p => p.Order.Id == orderId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Payments
            .Include(p => p.Order)
            .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public void Update(Payment payment)
    {
        _context.Entry(payment).State = EntityState.Modified;
    }

    public void Delete(Payment payment)
    {
        _context.Payments.Remove(payment);
    }
} 
