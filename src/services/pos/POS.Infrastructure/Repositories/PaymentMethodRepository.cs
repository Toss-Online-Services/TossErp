using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.BuyerAggregate;
using POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;

namespace TossErp.POS.Infrastructure.Repositories;

public class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly POSContext _context;

    public PaymentMethodRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<PaymentMethod> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
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

    public async Task<PaymentMethod> AddAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default)
    {
        await _context.PaymentMethods.AddAsync(paymentMethod, cancellationToken);
        return paymentMethod;
    }

    public void Update(PaymentMethod paymentMethod)
    {
        _context.Entry(paymentMethod).State = EntityState.Modified;
    }

    public void Delete(PaymentMethod paymentMethod)
    {
        _context.PaymentMethods.Remove(paymentMethod);
    }

    public async Task<PaymentMethod> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentMethods.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<PaymentMethod>> GetByStoreIdAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentMethods
            .Where(p => p.StoreId == storeId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }
} 
