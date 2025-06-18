using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.ProductAggregate;
using POS.Domain.SeedWork;
using System.Linq.Expressions;

namespace POS.Infrastructure.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly POSContext _context;

    public ProductCategoryRepository(POSContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<ProductCategory> AddAsync(ProductCategory category, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Categories.AddAsync(category, cancellationToken);
        return entry.Entity;
    }

    public async Task<ProductCategory> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ProductCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductCategory>> GetAsync(Expression<Func<ProductCategory, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<ProductCategory, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Categories.CountAsync(predicate, cancellationToken);
    }

    public async Task UpdateAsync(ProductCategory category, CancellationToken cancellationToken = default)
    {
        _context.Entry(category).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(ProductCategory category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Remove(category);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await GetByIdAsync(id, cancellationToken);
        if (category != null)
        {
            await DeleteAsync(category, cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<ProductCategory> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<ProductCategory>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .Where(c => c.ParentCategoryId == parentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductCategory>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .Where(c => c.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductCategory>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.SubCategories)
            .Where(c => c.ParentCategoryId == null)
            .ToListAsync(cancellationToken);
    }
} 
