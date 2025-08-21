using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TossErp.Accounts.Domain.Entities;
using TossErp.Accounts.Domain.Enums;
using TossErp.Accounts.Domain.SeedWork;
using TossErp.Accounts.Application.Common.Interfaces;
using TossErp.Accounts.Infrastructure.Data;
using TossErp.Shared.SeedWork;

namespace TossErp.Accounts.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation for Accounts domain entities
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class AccountsRepository<T> : IRepository<T> where T : class, IAggregateRoot
{
    protected readonly AccountsDbContext _context;

    public AccountsRepository(AccountsDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork UnitOfWork => _context;

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Set<T>().AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public virtual T Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return entity;
    }

    public virtual void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        return predicate == null
            ? await _context.Set<T>().CountAsync(cancellationToken)
            : await _context.Set<T>().CountAsync(predicate, cancellationToken);
    }
}

/// <summary>
/// Chart of Accounts repository implementation
/// </summary>
public class ChartOfAccountsRepository : AccountsRepository<ChartOfAccount>, IChartOfAccountsRepository
{
    public ChartOfAccountsRepository(AccountsDbContext context) : base(context) { }

    public async Task<ChartOfAccount?> GetByCodeAsync(string accountCode, CancellationToken cancellationToken = default)
    {
        return await _context.ChartOfAccounts
            .FirstOrDefaultAsync(a => a.AccountCode == accountCode, cancellationToken);
    }

    public async Task<IEnumerable<ChartOfAccount>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        return await _context.ChartOfAccounts
            .Where(a => a.ParentAccountId == parentId)
            .OrderBy(a => a.AccountCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ChartOfAccount>> GetByAccountTypeAsync(AccountType accountType, CancellationToken cancellationToken = default)
    {
        return await _context.ChartOfAccounts
            .Where(a => a.AccountTypeId == accountType.Id)
            .OrderBy(a => a.AccountCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ChartOfAccount>> GetActiveAccountsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ChartOfAccounts
            .Where(a => a.IsActive)
            .OrderBy(a => a.AccountCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AccountCodeExistsAsync(string accountCode, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.ChartOfAccounts.Where(a => a.AccountCode == accountCode);
        
        if (excludeId.HasValue)
        {
            query = query.Where(a => a.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}

/// <summary>
/// Customer repository implementation
/// </summary>
public class CustomerRepository : AccountsRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(AccountsDbContext context) : base(context) { }

    public async Task<Customer?> GetByCustomerNumberAsync(string customerNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Include(c => c.Contacts)
            .FirstOrDefaultAsync(c => c.CustomerNumber == customerNumber, cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Include(c => c.Contacts)
            .Where(c => c.Email == email)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetActiveCustomersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Where(c => c.IsActive)
            .OrderBy(c => c.CompanyName)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CustomerNumberExistsAsync(string customerNumber, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Customers.Where(c => c.CustomerNumber == customerNumber);
        
        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}

/// <summary>
/// Vendor repository implementation
/// </summary>
public class VendorRepository : AccountsRepository<Vendor>, IVendorRepository
{
    public VendorRepository(AccountsDbContext context) : base(context) { }

    public async Task<Vendor?> GetByVendorNumberAsync(string vendorNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Vendors
            .Include(v => v.Contacts)
            .FirstOrDefaultAsync(v => v.VendorNumber == vendorNumber, cancellationToken);
    }

    public async Task<IEnumerable<Vendor>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Vendors
            .Include(v => v.Contacts)
            .Where(v => v.Email == email)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Vendor>> GetActiveVendorsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Vendors
            .Where(v => v.IsActive)
            .OrderBy(v => v.CompanyName)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> VendorNumberExistsAsync(string vendorNumber, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Vendors.Where(v => v.VendorNumber == vendorNumber);
        
        if (excludeId.HasValue)
        {
            query = query.Where(v => v.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}

/// <summary>
/// Invoice repository implementation
/// </summary>
public class InvoiceRepository : AccountsRepository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(AccountsDbContext context) : base(context) { }

    public async Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Include(i => i.Lines)
            .Include(i => i.Payments)
            .Include(i => i.Customer)
            .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber, cancellationToken);
    }

    public async Task<IEnumerable<Invoice>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Include(i => i.Lines)
            .Where(i => i.CustomerId == customerId)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Invoices
            .Include(i => i.Customer)
            .Where(i => i.DueDate < today && i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Cancelled)
            .OrderBy(i => i.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> InvoiceNumberExistsAsync(string invoiceNumber, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Invoices.Where(i => i.InvoiceNumber == invoiceNumber);
        
        if (excludeId.HasValue)
        {
            query = query.Where(i => i.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalOutstandingAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Where(i => i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Cancelled)
            .SumAsync(i => i.Total.Amount - i.AmountPaid.Amount, cancellationToken);
    }
}

/// <summary>
/// Bill repository implementation
/// </summary>
public class BillRepository : AccountsRepository<Bill>, IBillRepository
{
    public BillRepository(AccountsDbContext context) : base(context) { }

    public async Task<Bill?> GetByBillNumberAsync(string billNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Bills
            .Include(b => b.Lines)
            .Include(b => b.Payments)
            .Include(b => b.Vendor)
            .FirstOrDefaultAsync(b => b.BillNumber == billNumber, cancellationToken);
    }

    public async Task<IEnumerable<Bill>> GetByVendorIdAsync(Guid vendorId, CancellationToken cancellationToken = default)
    {
        return await _context.Bills
            .Include(b => b.Lines)
            .Where(b => b.VendorId == vendorId)
            .OrderByDescending(b => b.BillDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Bill>> GetOverdueBillsAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Bills
            .Include(b => b.Vendor)
            .Where(b => b.DueDate < today && b.Status != BillStatus.Paid && b.Status != BillStatus.Cancelled)
            .OrderBy(b => b.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> BillNumberExistsAsync(string billNumber, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Bills.Where(b => b.BillNumber == billNumber);
        
        if (excludeId.HasValue)
        {
            query = query.Where(b => b.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalOutstandingAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Bills
            .Where(b => b.Status != BillStatus.Paid && b.Status != BillStatus.Cancelled)
            .SumAsync(b => b.Total.Amount - b.AmountPaid.Amount, cancellationToken);
    }
}

/// <summary>
/// Cashbook repository implementation
/// </summary>
public class CashbookRepository : AccountsRepository<Cashbook>, ICashbookRepository
{
    public CashbookRepository(AccountsDbContext context) : base(context) { }

    public async Task<Cashbook?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Cashbooks
            .Include(c => c.Entries)
            .FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    public async Task<Cashbook?> GetDefaultCashbookAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Cashbooks
            .Include(c => c.Entries)
            .FirstOrDefaultAsync(c => c.Name == "Main Cashbook" && c.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<Cashbook>> GetActiveCashbooksAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Cashbooks
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetCashBalanceAsync(Guid cashbookId, CancellationToken cancellationToken = default)
    {
        var cashbook = await _context.Cashbooks
            .Include(c => c.Entries)
            .FirstOrDefaultAsync(c => c.Id == cashbookId, cancellationToken);

        return cashbook?.GetCurrentBalance().Amount ?? 0;
    }
}

/// <summary>
/// Payment repository implementation
/// </summary>
public class PaymentRepository : AccountsRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(AccountsDbContext context) : base(context) { }

    public async Task<Payment?> GetByPaymentNumberAsync(string paymentNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Include(p => p.Lines)
            .FirstOrDefaultAsync(p => p.PaymentNumber == paymentNumber, cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Include(p => p.Lines)
            .Where(p => p.PaymentDate >= fromDate && p.PaymentDate <= toDate)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByPaymentMethodAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Where(p => p.PaymentMethod == paymentMethod)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> PaymentNumberExistsAsync(string paymentNumber, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Payments.Where(p => p.PaymentNumber == paymentNumber);
        
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}
