using Microsoft.AspNetCore.Mvc;
using TossErp.Inventory.API.DTOs;
using TossErp.Inventory.Domain.AggregatesModel.ItemAggregate;
using TossErp.Inventory.Domain.Enums;
using TossErp.Inventory.Infrastructure.Repositories;

namespace TossErp.Inventory.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemRepository _itemRepository;
        private readonly ILogger<ItemsController> _logger;

        public ItemsController(IItemRepository itemRepository, ILogger<ItemsController> logger)
        {
            _itemRepository = itemRepository ?? throw new ArgumentNullException(nameof(itemRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<ItemListDto>> GetItems(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] ItemType? itemType = null,
            [FromQuery] Guid? categoryId = null,
            [FromQuery] Guid? brandId = null,
            [FromQuery] Guid? supplierId = null)
        {
            try
            {
                var items = await _itemRepository.GetActiveItemsAsync();

                // Apply filters
                if (itemType.HasValue)
                {
                    items = items.Where(i => i.ItemType == itemType.Value);
                }

                if (categoryId.HasValue)
                {
                    items = items.Where(i => i.CategoryId == categoryId.Value);
                }

                if (brandId.HasValue)
                {
                    items = items.Where(i => i.BrandId == brandId.Value);
                }

                if (supplierId.HasValue)
                {
                    items = items.Where(i => i.SupplierId == supplierId.Value);
                }

                var totalCount = items.Count();

                var pagedItems = items
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(MapToDto)
                    .ToList();

                var result = new ItemListDto
                {
                    Items = pagedItems,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving items");
                return StatusCode(500, "An error occurred while retrieving items");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItem(Guid id)
        {
            try
            {
                var item = await _itemRepository.GetByIdAsync(id);
                if (item == null)
                {
                    return NotFound($"Item with ID {id} not found");
                }

                return Ok(MapToDto(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving item with ID {ItemId}", id);
                return StatusCode(500, "An error occurred while retrieving the item");
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

        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItem([FromBody] CreateItemDto createItemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if item code already exists
                if (await _itemRepository.ItemCodeExistsAsync(createItemDto.ItemCode))
                {
                    return BadRequest($"Item code '{createItemDto.ItemCode}' already exists");
                }

                // Check if barcode already exists
                if (!string.IsNullOrEmpty(createItemDto.Barcode) && 
                    await _itemRepository.BarcodeExistsAsync(createItemDto.Barcode))
                {
                    return BadRequest($"Barcode '{createItemDto.Barcode}' already exists");
                }

                // Check if SKU already exists
                if (!string.IsNullOrEmpty(createItemDto.SKU) && 
                    await _itemRepository.SKUExistsAsync(createItemDto.SKU))
                {
                    return BadRequest($"SKU '{createItemDto.SKU}' already exists");
                }

                var item = new Item(
                    createItemDto.ItemCode,
                    createItemDto.Name,
                    createItemDto.Description,
                    createItemDto.Barcode,
                    createItemDto.SKU,
                    createItemDto.ItemType,
                    createItemDto.IsStockable,
                    createItemDto.StandardCost,
                    createItemDto.SellingPrice,
                    createItemDto.UnitOfMeasure,
                    createItemDto.MinimumStockLevel,
                    createItemDto.MaximumStockLevel,
                    createItemDto.ReorderPoint,
                    createItemDto.ReorderQuantity,
                    createItemDto.CategoryId,
                    createItemDto.BrandId,
                    createItemDto.SupplierId
                );

                await _itemRepository.AddAsync(item);
                await _itemRepository.UnitOfWork.SaveChangesAsync();

                _logger.LogInformation("Created item with ID {ItemId}", item.Id);

                return CreatedAtAction(nameof(GetItem), new { id = item.Id }, MapToDto(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating item");
                return StatusCode(500, "An error occurred while creating the item");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ItemDto>> UpdateItem(Guid id, [FromBody] UpdateItemDto updateItemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = await _itemRepository.GetByIdAsync(id);
                if (item == null)
                {
                    return NotFound($"Item with ID {id} not found");
                }

                // Check if barcode already exists (if changed)
                if (!string.IsNullOrEmpty(updateItemDto.Barcode) && 
                    updateItemDto.Barcode != item.Barcode &&
                    await _itemRepository.BarcodeExistsAsync(updateItemDto.Barcode))
                {
                    return BadRequest($"Barcode '{updateItemDto.Barcode}' already exists");
                }

                // Check if SKU already exists (if changed)
                if (!string.IsNullOrEmpty(updateItemDto.SKU) && 
                    updateItemDto.SKU != item.SKU &&
                    await _itemRepository.SKUExistsAsync(updateItemDto.SKU))
                {
                    return BadRequest($"SKU '{updateItemDto.SKU}' already exists");
                }

                item.UpdateDetails(
                    updateItemDto.Name,
                    updateItemDto.Description,
                    updateItemDto.Barcode,
                    updateItemDto.SKU,
                    updateItemDto.StandardCost,
                    updateItemDto.SellingPrice,
                    updateItemDto.UnitOfMeasure,
                    updateItemDto.MinimumStockLevel,
                    updateItemDto.MaximumStockLevel,
                    updateItemDto.ReorderPoint,
                    updateItemDto.ReorderQuantity,
                    updateItemDto.CategoryId,
                    updateItemDto.BrandId,
                    updateItemDto.SupplierId
                );

                _itemRepository.Update(item);
                await _itemRepository.UnitOfWork.SaveChangesAsync();

                _logger.LogInformation("Updated item with ID {ItemId}", id);

                return Ok(MapToDto(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item with ID {ItemId}", id);
                return StatusCode(500, "An error occurred while updating the item");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItem(Guid id)
        {
            try
            {
                var item = await _itemRepository.GetByIdAsync(id);
                if (item == null)
                {
                    return NotFound($"Item with ID {id} not found");
                }

                item.Deactivate();
                _itemRepository.Update(item);
                await _itemRepository.UnitOfWork.SaveChangesAsync();

                _logger.LogInformation("Deactivated item with ID {ItemId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting item with ID {ItemId}", id);
                return StatusCode(500, "An error occurred while deleting the item");
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
                await _itemRepository.UnitOfWork.SaveChangesAsync();

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
                await _itemRepository.UnitOfWork.SaveChangesAsync();

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
                await _itemRepository.UnitOfWork.SaveChangesAsync();

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
