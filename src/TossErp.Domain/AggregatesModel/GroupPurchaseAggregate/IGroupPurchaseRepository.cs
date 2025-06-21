using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.GroupPurchaseAggregate;

public interface IGroupPurchaseRepository : IRepository<GroupPurchase>
{
    Task<GroupPurchase?> GetByIdAsync(Guid id);
    Task<GroupPurchase?> GetByGroupNumberAsync(string groupNumber);
    Task<IEnumerable<GroupPurchase>> GetByBusinessIdAsync(Guid businessId);
    Task<IEnumerable<GroupPurchase>> GetActiveGroupPurchasesAsync(Guid businessId);
    Task<IEnumerable<GroupPurchase>> GetByProductIdAsync(Guid productId);
    Task<IEnumerable<GroupPurchase>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<GroupPurchase>> GetExpiringSoonAsync(int daysThreshold = 3);
    Task<IEnumerable<GroupPurchase>> GetByStatusAsync(string status, Guid businessId);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> UserIsMemberAsync(Guid groupPurchaseId, Guid userId);
} 
