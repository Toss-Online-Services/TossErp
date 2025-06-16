using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using POS.Domain.Models;
using POS.Domain.Repositories;
using POS.Domain.Common;
using POS.Domain.SeedWork;
using TossErp.POS.Infrastructure.Data;

namespace TossErp.POS.Infrastructure.Repositories;

public class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly POSContext _context;

    public PaymentMethodRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<PaymentMethod?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentMethods.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<PaymentMethod>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PaymentMethods.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PaymentMethod>> FindAsync(Expression<Func<PaymentMethod, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentMethods.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default)
    {
        await _context.PaymentMethods.AddAsync(paymentMethod, cancellationToken);
    }

    public async Task UpdateAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default)
    {
        _context.Entry(paymentMethod).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default)
    {
        _context.PaymentMethods.Remove(paymentMethod);
        await Task.CompletedTask;
    }

    public async Task<PaymentMethod?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentMethods
            .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<PaymentMethod>> GetByTypeAsync(string type, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentMethods
            .Where(p => p.Type == type)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }
} 
