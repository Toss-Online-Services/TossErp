using TossErp.Shared.DTOs;
using TossErp.Shared.Enums;

namespace TossErp.Inventory.API.Services
{
    public class ItemService : IItemService
    {
        private readonly List<ItemDto> _items = new();
        private readonly List<StockMovementDto> _stockMovements = new();
        private readonly List<CategoryDto> _categories = new();
        private readonly ILogger<ItemService> _logger;

        public ItemService(ILogger<ItemService> logger)
        {
            _logger = logger;
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            // Initialize categories
            _categories.AddRange(new[]
            {
                new CategoryDto { Id = Guid.NewGuid(), Name = "Electronics", Description = "Electronic devices and accessories" },
                new CategoryDto { Id = Guid.NewGuid(), Name = "Clothing", Description = "Apparel and fashion items" },
                new CategoryDto { Id = Guid.NewGuid(), Name = "Books", Description = "Books and publications" },
                new CategoryDto { Id = Guid.NewGuid(), Name = "Home & Garden", Description = "Home improvement and garden items" }
            });

            // Initialize sample items
            _items.AddRange(new[]
            {
                new ItemDto
                {
                    Id = Guid.NewGuid(),
                    ItemCode = "LAPTOP001",
                    Name = "Dell Latitude Laptop",
                    Description = "15.6 inch business laptop with Intel i7 processor",
                    Barcode = "1234567890123",
                    SKU = "DELL-LAT-001",
                    ItemType = ItemType.Product,
                    IsStockable = true,
                    StandardCost = 800.00m,
                    SellingPrice = 1200.00m,
                    UnitOfMeasure = "Piece",
                    CurrentStock = 25,
                    MinimumStockLevel = 5,
                    MaximumStockLevel = 50,
                    ReorderPoint = 10,
                    ReorderQuantity = 20,
                    CategoryId = _categories[0].Id,
                    CategoryName = _categories[0].Name,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ItemDto
                {
                    Id = Guid.NewGuid(),
                    ItemCode = "MOUSE001",
                    Name = "Wireless Mouse",
                    Description = "Ergonomic wireless mouse with 2.4GHz connectivity",
                    Barcode = "1234567890124",
                    SKU = "MOUSE-WL-001",
                    ItemType = ItemType.Product,
                    IsStockable = true,
                    StandardCost = 15.00m,
                    SellingPrice = 25.00m,
                    UnitOfMeasure = "Piece",
                    CurrentStock = 8,
                    MinimumStockLevel = 10,
                    MaximumStockLevel = 100,
                    ReorderPoint = 15,
                    ReorderQuantity = 50,
                    CategoryId = _categories[0].Id,
                    CategoryName = _categories[0].Name,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ItemDto
                {
                    Id = Guid.NewGuid(),
                    ItemCode = "TSHIRT001",
                    Name = "Cotton T-Shirt",
                    Description = "100% cotton comfortable t-shirt",
                    Barcode = "1234567890125",
                    SKU = "TSHIRT-COT-001",
                    ItemType = ItemType.Product,
                    IsStockable = true,
                    StandardCost = 8.00m,
                    SellingPrice = 15.00m,
                    UnitOfMeasure = "Piece",
                    CurrentStock = 150,
                    MinimumStockLevel = 20,
                    MaximumStockLevel = 200,
                    ReorderPoint = 30,
                    ReorderQuantity = 100,
                    CategoryId = _categories[1].Id,
                    CategoryName = _categories[1].Name,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            });
        }

        public async Task<List<ItemDto>> GetItemsAsync(string? searchTerm, int page, int pageSize)
        {
            var query = _items.Where(i => i.IsActive);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(i => 
                    i.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    i.ItemCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    i.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                    i.Barcode?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                    i.SKU?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true);
            }

            return query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public async Task<ItemDto?> GetItemByIdAsync(Guid id)
        {
            return _items.FirstOrDefault(i => i.Id == id && i.IsActive);
        }

        public async Task<ItemDto> CreateItemAsync(CreateItemDto request)
        {
            var item = new ItemDto
            {
                Id = Guid.NewGuid(),
                ItemCode = request.ItemCode,
                Name = request.Name,
                Description = request.Description,
                Barcode = request.Barcode,
                SKU = request.SKU,
                ItemType = request.ItemType,
                IsStockable = request.IsStockable,
                StandardCost = request.StandardCost,
                SellingPrice = request.SellingPrice,
                UnitOfMeasure = request.UnitOfMeasure,
                CurrentStock = 0,
                MinimumStockLevel = request.MinimumStockLevel,
                MaximumStockLevel = request.MaximumStockLevel,
                ReorderPoint = request.ReorderPoint,
                ReorderQuantity = request.ReorderQuantity,
                CategoryId = request.CategoryId,
                CategoryName = request.CategoryId.HasValue ? 
                    _categories.FirstOrDefault(c => c.Id == request.CategoryId)?.Name : null,
                BrandId = request.BrandId,
                SupplierId = request.SupplierId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _items.Add(item);
            _logger.LogInformation("Created new item: {ItemCode}", item.ItemCode);
            return item;
        }

        public async Task<ItemDto?> UpdateItemAsync(Guid id, UpdateItemDto request)
        {
            var item = _items.FirstOrDefault(i => i.Id == id && i.IsActive);
            if (item == null)
            {
                return null;
            }

            item.Name = request.Name;
            item.Description = request.Description;
            item.Barcode = request.Barcode;
            item.SKU = request.SKU;
            item.ItemType = request.ItemType;
            item.IsStockable = request.IsStockable;
            item.StandardCost = request.StandardCost;
            item.SellingPrice = request.SellingPrice;
            item.UnitOfMeasure = request.UnitOfMeasure;
            item.MinimumStockLevel = request.MinimumStockLevel;
            item.MaximumStockLevel = request.MaximumStockLevel;
            item.ReorderPoint = request.ReorderPoint;
            item.ReorderQuantity = request.ReorderQuantity;
            item.CategoryId = request.CategoryId;
            item.CategoryName = request.CategoryId.HasValue ? 
                _categories.FirstOrDefault(c => c.Id == request.CategoryId)?.Name : null;
            item.BrandId = request.BrandId;
            item.SupplierId = request.SupplierId;
            item.LastModifiedAt = DateTime.UtcNow;

            _logger.LogInformation("Updated item: {ItemCode}", item.ItemCode);
            return item;
        }

        public async Task<bool> DeleteItemAsync(Guid id)
        {
            var item = _items.FirstOrDefault(i => i.Id == id && i.IsActive);
            if (item == null)
            {
                return false;
            }

            item.IsActive = false;
            item.LastModifiedAt = DateTime.UtcNow;
            _logger.LogInformation("Deleted item: {ItemCode}", item.ItemCode);
            return true;
        }

        public async Task<ItemDto?> AdjustStockAsync(Guid id, StockAdjustmentDto request)
        {
            var item = _items.FirstOrDefault(i => i.Id == id && i.IsActive);
            if (item == null)
            {
                return null;
            }

            var oldStock = item.CurrentStock;
            
            switch (request.MovementType)
            {
                case StockMovementType.In:
                    item.CurrentStock += request.Quantity;
                    break;
                case StockMovementType.Out:
                    if (item.CurrentStock < request.Quantity)
                    {
                        throw new InvalidOperationException("Insufficient stock for adjustment");
                    }
                    item.CurrentStock -= request.Quantity;
                    break;
                case StockMovementType.Adjustment:
                    item.CurrentStock = request.Quantity;
                    break;
            }

            // Record stock movement
            var movement = new StockMovementDto
            {
                Id = Guid.NewGuid(),
                ItemId = id,
                ItemName = item.Name,
                Quantity = request.Quantity,
                MovementType = request.MovementType,
                PreviousStock = oldStock,
                NewStock = item.CurrentStock,
                Reason = request.Reason,
                Reference = request.Reference,
                WarehouseId = request.WarehouseId,
                CreatedAt = DateTime.UtcNow
            };

            _stockMovements.Add(movement);
            item.LastModifiedAt = DateTime.UtcNow;

            _logger.LogInformation("Stock adjusted for item {ItemCode}: {OldStock} -> {NewStock}", 
                item.ItemCode, oldStock, item.CurrentStock);

            return item;
        }

        public async Task<List<StockMovementDto>> GetStockMovementsAsync(Guid id, DateTime? fromDate, DateTime? toDate)
        {
            var query = _stockMovements.Where(m => m.ItemId == id);

            if (fromDate.HasValue)
            {
                query = query.Where(m => m.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(m => m.CreatedAt <= toDate.Value);
            }

            return query.OrderByDescending(m => m.CreatedAt).ToList();
        }

        public async Task<List<ItemDto>> GetLowStockItemsAsync(int threshold)
        {
            return _items
                .Where(i => i.IsActive && i.IsStockable && i.CurrentStock <= threshold)
                .OrderBy(i => i.CurrentStock)
                .ToList();
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            return _categories.ToList();
        }
    }
} 
