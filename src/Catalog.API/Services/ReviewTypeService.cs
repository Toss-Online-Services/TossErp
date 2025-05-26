using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Services;

public class ReviewTypeService : IReviewTypeService
{
    private readonly CatalogContext _context;

    public ReviewTypeService(CatalogContext context)
    {
        _context = context;
    }

    public async Task<ReviewType?> GetReviewTypeByIdAsync(int reviewTypeId)
    {
        return await _context.ReviewTypes
            .FirstOrDefaultAsync(rt => rt.Id == reviewTypeId);
    }

    public async Task<IEnumerable<ReviewType>> GetAllReviewTypesAsync()
    {
        return await _context.ReviewTypes.ToListAsync();
    }

    public async Task<IEnumerable<ReviewType>> GetReviewTypesByNameAsync(string name)
    {
        return await _context.ReviewTypes
            .Where(rt => rt.Name.Contains(name))
            .ToListAsync();
    }

    public async Task<ReviewType> InsertReviewTypeAsync(ReviewType reviewType)
    {
        await _context.ReviewTypes.AddAsync(reviewType);
        await _context.SaveChangesAsync();
        return reviewType;
    }

    public async Task<ReviewType> UpdateReviewTypeAsync(ReviewType reviewType)
    {
        _context.ReviewTypes.Update(reviewType);
        await _context.SaveChangesAsync();
        return reviewType;
    }

    public async Task DeleteReviewTypeAsync(ReviewType reviewType)
    {
        _context.ReviewTypes.Remove(reviewType);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteReviewTypesAsync(IList<ReviewType> reviewTypes)
    {
        _context.ReviewTypes.RemoveRange(reviewTypes);
        await _context.SaveChangesAsync();
    }

    public async Task<IList<ReviewType>> GetAllReviewTypesAsync(bool showHidden = false)
    {
        var query = _context.ReviewTypes.AsQueryable();

        if (!showHidden)
        {
            query = query.Where(rt => rt.Published);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<ReviewType>> GetReviewTypesByNameAsync(string name, bool showHidden = false)
    {
        var query = _context.ReviewTypes
            .Where(rt => rt.Name.Contains(name));

        if (!showHidden)
        {
            query = query.Where(rt => rt.Published);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<ReviewType>> GetAllReviewTypesAsync(bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _context.ReviewTypes.AsQueryable();

        if (!showHidden)
        {
            query = query.Where(rt => rt.Published);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IList<ReviewType>> GetReviewTypesByNameAsync(string name, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _context.ReviewTypes
            .Where(rt => rt.Name.Contains(name));

        if (!showHidden)
        {
            query = query.Where(rt => rt.Published);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IList<ProductReview>> GetProductReviewsByReviewTypeIdAsync(int reviewTypeId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _context.ProductReviews
            .Where(pr => pr.ReviewTypeId == reviewTypeId);

        if (!showHidden)
        {
            query = query.Where(pr => pr.Published);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetProductReviewCountByReviewTypeIdAsync(int reviewTypeId, bool showHidden = false)
    {
        var query = _context.ProductReviews
            .Where(pr => pr.ReviewTypeId == reviewTypeId);

        if (!showHidden)
        {
            query = query.Where(pr => pr.Published);
        }

        return await query.CountAsync();
    }
} 
