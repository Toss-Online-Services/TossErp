using TossErp.Domain.SeedWork;
using TossErp.Inventory.Domain.AggregatesModel.ItemAggregate;
using TossErp.Inventory.Domain.Enums;

namespace TossErp.Inventory.Infrastructure.Repositories
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<Item?> GetByItemCodeAsync(string itemCode);
        Task<Item?> GetByBarcodeAsync(string barcode);
        Task<Item?> GetBySKUAsync(string sku);
        Task<IEnumerable<Item>> GetActiveItemsAsync();
        Task<IEnumerable<Item>> GetItemsByTypeAsync(ItemType itemType);
        Task<IEnumerable<Item>> GetItemsByCategoryAsync(Guid categoryId);
        Task<IEnumerable<Item>> GetItemsByBrandAsync(Guid brandId);
        Task<IEnumerable<Item>> GetItemsBySupplierAsync(Guid supplierId);
        Task<IEnumerable<Item>> GetItemsNeedingReorderAsync();
        Task<bool> ItemCodeExistsAsync(string itemCode);
        Task<bool> BarcodeExistsAsync(string barcode);
        Task<bool> SKUExistsAsync(string sku);
    }
} 
