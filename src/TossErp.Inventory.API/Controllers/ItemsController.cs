using Microsoft.AspNetCore.Mvc;
using TossErp.Inventory.API.DTOs;
using TossErp.Inventory.Domain.AggregatesModel.ItemAggregate;
using TossErp.Inventory.Domain.Enums;
using TossErp.Inventory.Infrastructure.Repositories;
using TossErp.Domain.SeedWork;
using TossErp.Shared.DTOs;
using TossErp.Inventory.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace TossErp.Inventory.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        private readonly IItemRepository _itemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ItemsController> _logger;
        private readonly IItemService _itemService;

        public ItemsController(IItemRepository itemRepository, IUnitOfWork unitOfWork, ILogger<ItemsController> logger, IItemService itemService)
        {
            _itemRepository = itemRepository ?? throw new ArgumentNullException(nameof(itemRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetItems([FromQuery] string? searchTerm, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var items = await _itemService.GetItemsAsync(searchTerm, page, pageSize);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving items");
                return StatusCode(500, new { message = "Failed to retrieve items", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(Guid id)
        {
            try
            {
                var item = await _itemService.GetItemByIdAsync(id);
                if (item == null)
                {
                    return NotFound();
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving item {ItemId}", id);
                return StatusCode(500, new { message = "Failed to retrieve item", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] CreateItemDto request)
        {
            try
            {
                var item = await _itemService.CreateItemAsync(request);
                return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating item");
                return BadRequest(new { message = "Failed to create item", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, [FromBody] UpdateItemDto request)
        {
            try
            {
                var item = await _itemService.UpdateItemAsync(id, request);
                if (item == null)
                {
                    return NotFound();
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item {ItemId}", id);
                return BadRequest(new { message = "Failed to update item", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            try
            {
                var success = await _itemService.DeleteItemAsync(id);
                if (!success)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting item {ItemId}", id);
                return StatusCode(500, new { message = "Failed to delete item", error = ex.Message });
            }
        }

        [HttpPost("{id}/adjust-stock")]
        public async Task<IActionResult> AdjustStock(Guid id, [FromBody] StockAdjustmentDto request)
        {
            try
            {
                var result = await _itemService.AdjustStockAsync(id, request);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adjusting stock for item {ItemId}", id);
                return BadRequest(new { message = "Failed to adjust stock", error = ex.Message });
            }
        }

        [HttpGet("{id}/stock-movements")]
        public async Task<IActionResult> GetStockMovements(Guid id, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            try
            {
                var movements = await _itemService.GetStockMovementsAsync(id, fromDate, toDate);
                return Ok(movements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stock movements for item {ItemId}", id);
                return StatusCode(500, new { message = "Failed to retrieve stock movements", error = ex.Message });
            }
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockItems([FromQuery] int threshold = 10)
        {
            try
            {
                var items = await _itemService.GetLowStockItemsAsync(threshold);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving low stock items");
                return StatusCode(500, new { message = "Failed to retrieve low stock items", error = ex.Message });
            }
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _itemService.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                return StatusCode(500, new { message = "Failed to retrieve categories", error = ex.Message });
            }
        }

        [HttpGet("code/{itemCode}")]
        public async Task<ActionResult<ItemDto>> GetItemByCode(string itemCode)
        {
            try
            {
                var item = await _itemRepository.GetByItemCodeAsync(itemCode);
                if (item == null)
                {
                    return NotFound($"Item with code '{itemCode}' not found");
                }

                return Ok(MapToDto(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving item with code {ItemCode}", itemCode);
                return StatusCode(500, "An error occurred while retrieving the item");
            }
        }

        [HttpGet("barcode/{barcode}")]
        public async Task<ActionResult<ItemDto>> GetItemByBarcode(string barcode)
        {
            try
            {
                var item = await _itemRepository.GetByBarcodeAsync(barcode);
                if (item == null)
                {
                    return NotFound($"Item with barcode '{barcode}' not found");
                }

                return Ok(MapToDto(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving item with barcode {Barcode}", barcode);
                return StatusCode(500, "An error occurred while retrieving the item");
            }
        }

        [HttpPost("{id}/activate")]
        public async Task<ActionResult<ItemDto>> ActivateItem(Guid id)
        {
            try
            {
                var item = await _itemRepository.GetByIdAsync(id);
                if (item == null)
                {
                    return NotFound($"Item with ID {id} not found");
                }

                item.Activate();
                _itemRepository.Update(item);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Activated item with ID {ItemId}", id);

                return Ok(MapToDto(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating item with ID {ItemId}", id);
                return StatusCode(500, "An error occurred while activating the item");
            }
        }

        [HttpPost("{id}/variants")]
        public async Task<ActionResult<ItemDto>> AddVariant(Guid id, [FromBody] string variantName)
        {
            try
            {
                var item = await _itemRepository.GetByIdAsync(id);
                if (item == null)
                {
                    return NotFound($"Item with ID {id} not found");
                }

                item.AddVariant(variantName);
                _itemRepository.Update(item);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Added variant {VariantName} to item with ID {ItemId}", variantName, id);

                return Ok(MapToDto(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding variant to item with ID {ItemId}", id);
                return StatusCode(500, "An error occurred while adding the variant");
            }
        }

        [HttpDelete("{id}/variants/{variantId}")]
        public async Task<ActionResult<ItemDto>> RemoveVariant(Guid id, Guid variantId)
        {
            try
            {
                var item = await _itemRepository.GetByIdAsync(id);
                if (item == null)
                {
                    return NotFound($"Item with ID {id} not found");
                }

                item.RemoveVariant(variantId);
                _itemRepository.Update(item);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Removed variant {VariantId} from item with ID {ItemId}", variantId, id);

                return Ok(MapToDto(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing variant from item with ID {ItemId}", id);
                return StatusCode(500, "An error occurred while removing the variant");
            }
        }

        [HttpGet("reorder")]
        public async Task<ActionResult<List<ItemDto>>> GetItemsNeedingReorder()
        {
            try
            {
                var items = await _itemRepository.GetItemsNeedingReorderAsync();
                var itemDtos = items.Select(MapToDto).ToList();

                return Ok(itemDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving items needing reorder");
                return StatusCode(500, "An error occurred while retrieving items needing reorder");
            }
        }

        private static ItemDto MapToDto(Item item)
        {
            return new ItemDto
            {
                Id = item.Id,
                ItemCode = item.ItemCode,
                Name = item.Name,
                Description = item.Description,
                Barcode = item.Barcode,
                SKU = item.SKU,
                ItemType = item.ItemType,
                IsActive = item.IsActive,
                IsStockable = item.IsStockable,
                IsSerialized = item.IsSerialized,
                IsBatched = item.IsBatched,
                StandardCost = item.StandardCost,
                SellingPrice = item.SellingPrice,
                UnitOfMeasure = item.UnitOfMeasure,
                MinimumStockLevel = item.MinimumStockLevel,
                MaximumStockLevel = item.MaximumStockLevel,
                ReorderPoint = item.ReorderPoint,
                ReorderQuantity = item.ReorderQuantity,
                CategoryId = item.CategoryId,
                BrandId = item.BrandId,
                SupplierId = item.SupplierId,
                CreatedAt = item.CreatedAt,
                LastModifiedAt = item.LastModifiedAt,
                Variants = item.Variants.Select(v => new ItemVariantDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    VariantCode = v.VariantCode,
                    IsActive = v.IsActive,
                    CreatedAt = v.CreatedAt
                }).ToList(),
                PriceHistory = item.PriceHistory.Select(ph => new ItemPriceHistoryDto
                {
                    Id = ph.Id,
                    Price = ph.Price,
                    EffectiveDate = ph.EffectiveDate,
                    Reason = ph.Reason
                }).ToList(),
                CurrentPrice = item.SellingPrice
            };
        }
    }
} 
