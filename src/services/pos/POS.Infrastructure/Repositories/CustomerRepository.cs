#nullable enable

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.CustomerAggregate;
using POS.Domain.SeedWork;
using POS.Infrastructure.EntityConfigurations;

namespace POS.Infrastructure.Repositories;

public class CustomerRepository : IRepository<Customer>
{
    private readonly POSContext _context;

    public CustomerRepository(POSContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Customers.AddAsync(customer, cancellationToken);
        return entry.Entity;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Include(c => c.PriceLists)
            .Include(c => c.Contacts)
            .Include(c => c.Documents)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Include(c => c.PriceLists)
            .Include(c => c.Contacts)
            .Include(c => c.Documents)
            .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetAsync(Expression<Func<Customer, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Include(c => c.PriceLists)
            .Include(c => c.Contacts)
            .Include(c => c.Documents)
            .Where(predicate)
            .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _context.Entry(customer).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _context.Customers.Remove(customer);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _context.Customers.FindAsync(new object[] { id }, cancellationToken);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Customer, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Customers.CountAsync(predicate, cancellationToken);
    }

    // Overloads without CancellationToken
    public Task<Customer> AddAsync(Customer customer) => AddAsync(customer, default);
    public Task UpdateAsync(Customer customer) => UpdateAsync(customer, default);
    public Task DeleteAsync(Customer customer) => DeleteAsync(customer, default);

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _context.Customers
            .Include(c => c.PriceLists)
            .Include(c => c.Contacts)
            .Include(c => c.Documents)
            .FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<Customer?> GetByPhoneAsync(string phone)
    {
        return await _context.Customers
            .Include(c => c.PriceLists)
            .Include(c => c.Contacts)
            .Include(c => c.Documents)
            .FirstOrDefaultAsync(c => c.PhoneNumber == phone);
    }

    public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
    {
        return await _context.Customers
            .Include(c => c.PriceLists)
            .Include(c => c.Contacts)
            .Include(c => c.Documents)
            .Where(c => c.IsActive)
            .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetByCreditLimitAsync(decimal minCreditLimit)
    {
        return await _context.Customers
            .Include(c => c.PriceLists)
            .Include(c => c.Contacts)
            .Include(c => c.Documents)
            .Where(c => c.CreditLimit >= minCreditLimit)
            .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
            .ToListAsync();
    }

    public void Update(Customer customer)
    {
        _context.Entry(customer).State = EntityState.Modified;
    }

    public void Delete(Customer customer)
    {
        _context.Customers.Remove(customer);
    }
} 
