using TossErp.Application.DTOs;

namespace TossErp.Application.Services;

public interface IGroupPurchaseService
{
    Task<GroupPurchaseDto> GetByIdAsync(Guid id);
    Task<IEnumerable<GroupPurchaseDto>> GetAllAsync();
    Task<IEnumerable<GroupPurchaseDto>> GetByBusinessAsync(Guid businessId);
    Task<IEnumerable<GroupPurchaseDto>> GetActiveGroupsAsync(int count);
    Task<GroupPurchaseDto> CreateAsync(CreateGroupPurchaseDto createGroupPurchaseDto);
    Task<GroupPurchaseDto> UpdateAsync(Guid id, CreateGroupPurchaseDto updateGroupPurchaseDto);
    Task DeleteAsync(Guid id);
    Task<int> GetActiveGroupsCountAsync();
    Task AddMemberAsync(Guid groupId, Guid userId, int quantity, string? notes = null);
    Task RemoveMemberAsync(Guid groupId, Guid userId);
    Task UpdateMemberQuantityAsync(Guid groupId, Guid userId, int quantity);
} 
