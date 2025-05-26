using Catalog.Domain.DTOs;

namespace Catalog.Domain.Services;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetReviewsAsync(int catalogItemId, int page = 1, int pageSize = 10);
    Task<ReviewDto> AddReviewAsync(ReviewDto review);
    Task<ReviewDto> UpdateReviewAsync(int reviewId, ReviewDto review);
    Task DeleteReviewAsync(int reviewId);
    Task<double> GetAverageRatingAsync(int catalogItemId);
    Task VoteReviewAsync(int reviewId, bool isHelpful);
    Task<bool> CanUserReviewAsync(string userId, int catalogItemId);
} 
