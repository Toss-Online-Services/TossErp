using TossErp.Inventory.API.DTOs;
using TossErp.Inventory.Domain.Enums;

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
                    CurrentPrice = 1200.00m,
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
                    CurrentPrice = 25.00m,
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
                    CurrentPrice = 15.00m,
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

        public Task<List<ItemDto>> GetItemsAsync(string? searchTerm, int page, int pageSize)
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

            return Task.FromResult(query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList());
        }

        public Task<ItemDto?> GetItemByIdAsync(Guid id)
        {
            return Task.FromResult(_items.FirstOrDefault(i => i.Id == id && i.IsActive));
        }

        public Task<ItemDto> CreateItemAsync(CreateItemDto request)
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
                IsSerialized = request.IsSerialized,
                IsBatched = request.IsBatched,
                StandardCost = request.StandardCost,
                SellingPrice = request.SellingPrice,
                CurrentPrice = request.SellingPrice,
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
            return Task.FromResult(item);
        }

        public Task<ItemDto?> UpdateItemAsync(Guid id, UpdateItemDto request)
        {
            var item = _items.FirstOrDefault(i => i.Id == id && i.IsActive);
            if (item == null)
            {
                return Task.FromResult<ItemDto?>(null);
            }

            item.Name = request.Name;
            item.Description = request.Description;
            item.Barcode = request.Barcode;
            item.SKU = request.SKU;
            item.ItemType = request.ItemType;
            item.IsStockable = request.IsStockable;
            item.IsSerialized = request.IsSerialized;
            item.IsBatched = request.IsBatched;
            item.StandardCost = request.StandardCost;
            item.SellingPrice = request.SellingPrice;
            item.CurrentPrice = request.SellingPrice;
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
            item.IsActive = request.IsActive;
            item.LastModifiedAt = DateTime.UtcNow;

            _logger.LogInformation("Updated item: {ItemCode}", item.ItemCode);
            return Task.FromResult<ItemDto?>(item);
        }

        public Task<bool> DeleteItemAsync(Guid id)
        {
            var item = _items.FirstOrDefault(i => i.Id == id && i.IsActive);
            if (item == null)
            {
                return Task.FromResult(false);
            }

            item.IsActive = false;
            item.LastModifiedAt = DateTime.UtcNow;
            _logger.LogInformation("Deleted item: {ItemCode}", item.ItemCode);
            return Task.FromResult(true);
        }

        public Task<ItemDto?> AdjustStockAsync(Guid id, StockAdjustmentDto request)
        {
            var item = _items.FirstOrDefault(i => i.Id == id && i.IsActive);
            if (item == null)
            {
                return Task.FromResult<ItemDto?>(null);
            }

            var previousStock = item.CurrentStock;
            var quantity = (int)request.Quantity;

            switch (request.AdjustmentType)
            {
                case StockMovementType.In:
                    item.CurrentStock += quantity;
                    break;
                case StockMovementType.Out:
                    item.CurrentStock -= quantity;
                    break;
                case StockMovementType.Adjustment:
                    item.CurrentStock = quantity;
                    break;
            }

            // Create stock movement record
            var movement = new StockMovementDto
            {
                Id = Guid.NewGuid(),
                ItemId = item.Id,
                ItemName = item.Name,
                MovementType = request.AdjustmentType,
                Quantity = request.Quantity,
                UnitCost = request.UnitCost,
                Reference = request.Reference,
                Notes = request.Reason,
                MovementDate = DateTime.UtcNow,
                CreatedBy = Guid.Empty // TODO: Get from current user context
            };

            _stockMovements.Add(movement);
            item.LastModifiedAt = DateTime.UtcNow;

            _logger.LogInformation("Adjusted stock for item {ItemCode}: {PreviousStock} -> {NewStock}", 
                item.ItemCode, previousStock, item.CurrentStock);

            return Task.FromResult<ItemDto?>(item);
        }

        public Task<List<StockMovementDto>> GetStockMovementsAsync(Guid id, DateTime? fromDate, DateTime? toDate)
        {
            var query = _stockMovements.Where(m => m.ItemId == id);

            if (fromDate.HasValue)
            {
                query = query.Where(m => m.MovementDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(m => m.MovementDate <= toDate.Value);
            }

            return Task.FromResult(query
                .OrderByDescending(m => m.MovementDate)
                .ToList());
        }

        public Task<List<ItemDto>> GetLowStockItemsAsync(int threshold)
        {
            return Task.FromResult(_items
                .Where(i => i.IsActive && i.CurrentStock <= threshold)
                .ToList());
        }

        public Task<List<CategoryDto>> GetCategoriesAsync()
        {
            return Task.FromResult(_categories.ToList());
        }
    }
} 
