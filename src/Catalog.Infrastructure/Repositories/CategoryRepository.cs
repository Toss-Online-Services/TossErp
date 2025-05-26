using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly CatalogContext _context;

    public CategoryRepository(CatalogContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetAsync(int id)
    {
        return await _context.Categories
            .Include(c => c.Parent)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories
            .Include(c => c.Parent)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetByParentAsync(int parentId)
    {
        return await _context.Categories
            .Include(c => c.Parent)
            .Where(c => c.ParentId == parentId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetPublishedAsync()
    {
        return await _context.Categories
            .Include(c => c.Parent)
            .Where(c => c.IsPublished)
            .ToListAsync();
    }

    public async Task<Category> AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteAsync(int categoryId)
    {
        var category = await _context.Categories.FindAsync(categoryId);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int categoryId)
    {
        return await _context.Categories.AnyAsync(c => c.Id == categoryId);
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Categories.CountAsync();
    }
} 
