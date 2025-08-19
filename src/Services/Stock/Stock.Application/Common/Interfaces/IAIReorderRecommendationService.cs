using TossErp.Stock.Application.Common.Models;

namespace TossErp.Stock.Application.Common.Interfaces;

/// <summary>
/// Interface for AI-powered reorder recommendation service
/// </summary>
public interface IAIReorderRecommendationService
{
    /// <summary>
    /// Get AI-powered reorder recommendations for all items
    /// </summary>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of reorder recommendations</returns>
    Task<List<ReorderRecommendationDto>> GetReorderRecommendationsAsync(
        string tenantId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get AI-powered reorder recommendation for a specific item
    /// </summary>
    /// <param name="itemId">Item ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Reorder recommendation or null if not needed</returns>
    Task<ReorderRecommendationDto?> GetItemReorderRecommendationAsync(
        Guid itemId, 
        string tenantId, 
        CancellationToken cancellationToken = default);
}
