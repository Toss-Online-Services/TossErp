using Microsoft.EntityFrameworkCore;
using TossErp.Domain.SeedWork;
using TossErp.Inventory.Domain.AggregatesModel.ItemAggregate;
using TossErp.Inventory.Domain.Enums;
using TossErp.Inventory.Infrastructure.Data;

namespace TossErp.Inventory.Infrastructure.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly InventoryDbContext _context;

        public ItemRepository(InventoryDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Item?> GetByIdAsync(Guid id)
        {
            return await _context.Items
                .Include(i => i.Variants)
                .Include(i => i.PriceHistory)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Item?> GetByItemCodeAsync(string itemCode)
        {
            return await _context.Items
                .Include(i => i.Variants)
                .Include(i => i.PriceHistory)
                .FirstOrDefaultAsync(i => i.ItemCode == itemCode);
        }

        public async Task<Item?> GetByBarcodeAsync(string barcode)
        {
            return await _context.Items
                .Include(i => i.Variants)
                .Include(i => i.PriceHistory)
                .FirstOrDefaultAsync(i => i.Barcode == barcode);
        }

        public async Task<Item?> GetBySKUAsync(string sku)
        {
            return await _context.Items
                .Include(i => i.Variants)
                .Include(i => i.PriceHistory)
                .FirstOrDefaultAsync(i => i.SKU == sku);
        }

        public async Task<IEnumerable<Item>> GetActiveItemsAsync()
        {
            return await _context.Items
                .Include(i => i.Variants)
                .Include(i => i.PriceHistory)
                .Where(i => i.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetItemsByTypeAsync(ItemType itemType)
        {
            return await _context.Items
                .Include(i => i.Variants)
                .Include(i => i.PriceHistory)
                .Where(i => i.ItemType == itemType && i.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetItemsByCategoryAsync(Guid categoryId)
        {
            return await _context.Items
                .Include(i => i.Variants)
                .Include(i => i.PriceHistory)
                .Where(i => i.CategoryId == categoryId && i.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetItemsByBrandAsync(Guid brandId)
        {
            return await _context.Items
                .Include(i => i.Variants)
                .Include(i => i.PriceHistory)
                .Where(i => i.BrandId == brandId && i.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetItemsBySupplierAsync(Guid supplierId)
        {
            return await _context.Items
                .Include(i => i.Variants)
                .Include(i => i.PriceHistory)
                .Where(i => i.SupplierId == supplierId && i.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetItemsNeedingReorderAsync()
        {
            return await _context.Items
                .Include(i => i.Variants)
                .Include(i => i.PriceHistory)
                .Where(i => i.IsStockable && i.IsActive && i.ReorderPoint.HasValue)
                .ToListAsync();
        }

        public async Task<bool> ItemCodeExistsAsync(string itemCode)
        {
            return await _context.Items.AnyAsync(i => i.ItemCode == itemCode);
        }

        public async Task<bool> BarcodeExistsAsync(string barcode)
        {
            return await _context.Items.AnyAsync(i => i.Barcode == barcode);
        }

        public async Task<bool> SKUExistsAsync(string sku)
        {
            return await _context.Items.AnyAsync(i => i.SKU == sku);
        }

        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            return await _context.Items
                .Include(i => i.Variants)
                .Include(i => i.PriceHistory)
                .ToListAsync();
        }

        public async Task AddAsync(Item item)
        {
            await _context.Items.AddAsync(item);
        }

        public void Update(Item item)
        {
            _context.Entry(item).State = EntityState.Modified;
        }

        public void Delete(Item item)
        {
            _context.Items.Remove(item);
        }
    }
} 
