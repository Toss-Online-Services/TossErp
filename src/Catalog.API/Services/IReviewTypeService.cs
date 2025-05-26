using Catalog.Domain.Entities;

namespace Catalog.API.Services;

public interface IReviewTypeService
{
    Task<ReviewType?> GetReviewTypeByIdAsync(int reviewTypeId);
    Task<IEnumerable<ReviewType>> GetAllReviewTypesAsync();
    Task<IEnumerable<ReviewType>> GetReviewTypesByNameAsync(string name);
    Task<ReviewType> InsertReviewTypeAsync(ReviewType reviewType);
    Task<ReviewType> UpdateReviewTypeAsync(ReviewType reviewType);
    Task DeleteReviewTypeAsync(ReviewType reviewType);
    Task DeleteReviewTypesAsync(IList<ReviewType> reviewTypes);
    Task<IList<ReviewType>> GetAllReviewTypesAsync(bool showHidden = false);
    Task<IList<ReviewType>> GetReviewTypesByNameAsync(string name, bool showHidden = false);
    Task<IList<ReviewType>> GetAllReviewTypesAsync(bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<IList<ReviewType>> GetReviewTypesByNameAsync(string name, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<IList<ProductReview>> GetProductReviewsByReviewTypeIdAsync(int reviewTypeId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<int> GetProductReviewCountByReviewTypeIdAsync(int reviewTypeId, bool showHidden = false);
} 
