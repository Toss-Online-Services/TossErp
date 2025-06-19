using TossErp.Shared.DTOs;

namespace TossErp.Inventory.API.Services
{
    public interface IItemService
    {
        Task<List<ItemDto>> GetItemsAsync(string? searchTerm, int page, int pageSize);
        Task<ItemDto?> GetItemByIdAsync(Guid id);
        Task<ItemDto> CreateItemAsync(CreateItemDto request);
        Task<ItemDto?> UpdateItemAsync(Guid id, UpdateItemDto request);
        Task<bool> DeleteItemAsync(Guid id);
        Task<ItemDto?> AdjustStockAsync(Guid id, StockAdjustmentDto request);
        Task<List<StockMovementDto>> GetStockMovementsAsync(Guid id, DateTime? fromDate, DateTime? toDate);
        Task<List<ItemDto>> GetLowStockItemsAsync(int threshold);
        Task<List<CategoryDto>> GetCategoriesAsync();
    }
} 
