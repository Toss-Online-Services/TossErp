#nullable enable

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.ProductAggregate;
using POS.Domain.SeedWork;
using POS.Infrastructure.EntityConfigurations;

namespace POS.Infrastructure.Repositories;

public class ProductRepository : IRepository<Product>
{
    private readonly POSContext _context;

    public ProductRepository(POSContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Products.AddAsync(product, cancellationToken);
        return entry.Entity;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetAsync(Specification<Product> specification, CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsQueryable();
        if (specification != null)
        {
            var predicate = specification.ToExpression();
            query = query.Where(predicate);
        }
        return await query.OrderBy(p => p.Name).ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Entry(product).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Remove(product);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);
        if (product != null)
        {
            _context.Products.Remove(product);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(Specification<Product> specification, CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsQueryable();
        if (specification != null)
        {
            var predicate = specification.ToExpression();
            query = query.Where(predicate);
        }
        return await query.CountAsync(cancellationToken);
    }

    // Overloads without CancellationToken
    public Task<Product> AddAsync(Product product) => AddAsync(product, default);
    public Task UpdateAsync(Product product) => UpdateAsync(product, default);
    public Task DeleteAsync(Product product) => DeleteAsync(product, default);
} 
