#nullable enable

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.OrderAggregate;
using POS.Domain.SeedWork;
using POS.Infrastructure.EntityConfigurations;

namespace POS.Infrastructure.Repositories;

public class OrderRepository : IRepository<Order>
{
    private readonly POSContext _context;

    public OrderRepository(POSContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Orders.AddAsync(order, cancellationToken);
        return entry.Entity;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetAsync(Specification<Order> specification, CancellationToken cancellationToken = default)
    {
        var query = _context.Orders.AsQueryable();
        if (specification != null)
        {
            var predicate = specification.ToExpression();
            query = query.Where(predicate);
        }
        return await query.OrderByDescending(o => o.OrderDate).ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        _context.Entry(order).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Order order, CancellationToken cancellationToken = default)
    {
        _context.Orders.Remove(order);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders.FindAsync(new object[] { id }, cancellationToken);
        if (order != null)
        {
            _context.Orders.Remove(order);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .AnyAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(Specification<Order> specification, CancellationToken cancellationToken = default)
    {
        var query = _context.Orders.AsQueryable();
        if (specification != null)
        {
            var predicate = specification.ToExpression();
            query = query.Where(predicate);
        }
        return await query.CountAsync(cancellationToken);
    }

    // Overloads without CancellationToken
    public Task<Order> AddAsync(Order order) => AddAsync(order, default);
    public Task UpdateAsync(Order order) => UpdateAsync(order, default);
    public Task DeleteAsync(Order order) => DeleteAsync(order, default);

    public async Task<Order> GetAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Store)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        return order;
    }

    public async Task<Order> GetByOrderNumberAsync(string orderNumber)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Store)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

        return order;
    }

    public async Task<IEnumerable<Order>> GetByCustomerAsync(int customerId)
    {
        return await _context.Orders
            .Include(o => o.Store)
            .Where(o => o.Customer.Id == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByStoreAsync(int storeId)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Where(o => o.Store.Id == storeId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Store)
            .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public void Update(Order order)
    {
        _context.Entry(order).State = EntityState.Modified;
    }

    public void Delete(Order order)
    {
        _context.Orders.Remove(order);
    }
} 
