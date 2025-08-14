using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.DTOs;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace TossErp.Stock.Application.Items.Queries.GetLowStockItems;

/// <summary>
/// Handler for retrieving items that are below their reorder level
/// </summary>
public class GetLowStockItemsQueryHandler : IRequestHandler<GetLowStockItemsQuery, GetLowStockItemsResponse>
{
    private readonly IItemRepository _itemRepository;
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILogger<GetLowStockItemsQueryHandler> _logger;

    public GetLowStockItemsQueryHandler(
        IItemRepository itemRepository,
        IStockLevelRepository stockLevelRepository,
        IWarehouseRepository warehouseRepository,
        ILogger<GetLowStockItemsQueryHandler> logger)
    {
        _itemRepository = itemRepository;
        _stockLevelRepository = stockLevelRepository;
        _warehouseRepository = warehouseRepository;
        _logger = logger;
    }

    public async Task<GetLowStockItemsResponse> Handle(GetLowStockItemsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving low stock items with filters: WarehouseId={WarehouseId}, OutOfStockOnly={OutOfStockOnly}, CriticalOnly={CriticalOnly}",
            request.WarehouseId, request.OutOfStockOnly, request.CriticalOnly);

        // Build query to get items with their stock levels using a join
        var query = from item in _itemRepository.GetQueryable()
                   join stockLevel in _stockLevelRepository.GetQueryable() on item.Id equals stockLevel.ItemId
                   where item.IsStockItem && !item.Disabled && !item.Deleted
                   select new { Item = item, StockLevel = stockLevel };

        // Apply item filters
        if (!string.IsNullOrWhiteSpace(request.ItemGroup))
        {
            query = query.Where(x => x.Item.ItemGroup == request.ItemGroup);
        }

        if (!string.IsNullOrWhiteSpace(request.ItemType))
        {
            if (Enum.TryParse<ItemType>(request.ItemType, true, out var itemType))
            {
                query = query.Where(x => x.Item.ItemType == itemType);
            }
        }

        // Apply warehouse filter
        if (request.WarehouseId.HasValue)
        {
            query = query.Where(x => x.StockLevel.WarehouseId == request.WarehouseId.Value);
        }

        // Get items and their stock levels
        var itemStockLevels = await query.ToListAsync(cancellationToken);

        // Filter by stock levels and calculate urgency
        var lowStockItems = new List<LowStockItemDto>();

        foreach (var itemStockLevel in itemStockLevels)
        {
            var item = itemStockLevel.Item;
            var stockLevel = itemStockLevel.StockLevel;
            
            var currentStock = stockLevel.Quantity;
            var reorderLevel = item.ReOrderLevel ?? 0;
            var maxStock = item.MaxQty ?? 0;

            // Determine if this item qualifies as low stock
            var isLowStock = currentStock <= reorderLevel;
            var isOutOfStock = currentStock <= 0;
            var isCritical = currentStock <= (reorderLevel * 0.2m); // 20% of reorder level

            if (!isLowStock && !isOutOfStock) continue;

            // Apply filters
            if (request.OutOfStockOnly == true && !isOutOfStock) continue;
            if (request.CriticalOnly == true && !isCritical) continue;

            var urgencyLevel = DetermineUrgencyLevel(currentStock, reorderLevel, maxStock);
            var stockDeficit = Math.Max(0, reorderLevel - currentStock);

            lowStockItems.Add(new LowStockItemDto
            {
                Id = item.Id,
                Name = item.ItemName,
                Code = item.ItemCode.Value,
                CurrentStock = currentStock,
                ReorderLevel = reorderLevel,
                MaxStock = maxStock,
                StockDeficit = stockDeficit,
                UrgencyLevel = urgencyLevel,
                WarehouseName = stockLevel.Warehouse.Name,
                LastMovementDate = stockLevel.LastMovementDate
            });
        }

        // Apply sorting
        lowStockItems = ApplySorting(lowStockItems, request.SortBy, request.SortOrder).ToList();

        // Apply pagination
        var page = request.Page ?? 1;
        var pageSize = request.PageSize ?? 20;
        var skip = (page - 1) * pageSize;

        var totalCount = lowStockItems.Count;
        var pagedItems = lowStockItems.Skip(skip).Take(pageSize).ToList();

        // Calculate summary statistics
        var summary = new LowStockSummary
        {
            CriticalCount = lowStockItems.Count(i => i.UrgencyLevel == "Critical"),
            HighCount = lowStockItems.Count(i => i.UrgencyLevel == "High"),
            MediumCount = lowStockItems.Count(i => i.UrgencyLevel == "Medium"),
            LowCount = lowStockItems.Count(i => i.UrgencyLevel == "Low"),
            OutOfStockCount = lowStockItems.Count(i => i.CurrentStock <= 0)
        };

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        _logger.LogInformation("Retrieved {Count} low stock items out of {TotalCount} total", 
            pagedItems.Count, totalCount);

        return new GetLowStockItemsResponse
        {
            Items = pagedItems,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            Summary = summary
        };
    }

    private static LowStockItemDto CreateLowStockItemDto(ItemAggregate item, StockLevel? stockLevel, decimal currentStock, string urgencyLevel)
    {
        return new LowStockItemDto
        {
            Id = item.Id,
            Name = item.ItemName,
            Code = item.ItemCode.Value,
            CurrentStock = currentStock,
            ReorderLevel = item.ReOrderLevel ?? 0,
            MaxStock = item.MaxQty ?? 0,
            StockDeficit = Math.Max(0, (item.ReOrderLevel ?? 0) - currentStock),
            UrgencyLevel = urgencyLevel,
            WarehouseName = stockLevel?.Warehouse.Name ?? "No Warehouse",
            LastMovementDate = stockLevel?.LastMovementDate
        };
    }

    private static string DetermineUrgencyLevel(decimal currentStock, decimal reorderLevel, decimal maxStock)
    {
        if (currentStock <= 0) return "Critical";
        if (currentStock <= reorderLevel * 0.2m) return "Critical";
        if (currentStock <= reorderLevel * 0.5m) return "High";
        if (currentStock <= reorderLevel) return "Medium";
        return "Low";
    }

    private static IEnumerable<LowStockItemDto> ApplySorting(IEnumerable<LowStockItemDto> items, string? sortBy, string? sortOrder)
    {
        var isDescending = sortOrder?.ToLower() == "desc";

        return sortBy?.ToLower() switch
        {
            "itemname" => isDescending ? items.OrderByDescending(i => i.Name) : items.OrderBy(i => i.Name),
            "currentstock" => isDescending ? items.OrderByDescending(i => i.CurrentStock) : items.OrderBy(i => i.CurrentStock),
            "reorderlevel" => isDescending ? items.OrderByDescending(i => i.ReorderLevel) : items.OrderBy(i => i.ReorderLevel),
            "urgency" => isDescending ? items.OrderByDescending(i => GetUrgencyPriority(i.UrgencyLevel)) : items.OrderBy(i => GetUrgencyPriority(i.UrgencyLevel)),
            _ => isDescending ? items.OrderByDescending(i => GetUrgencyPriority(i.UrgencyLevel)) : items.OrderBy(i => GetUrgencyPriority(i.UrgencyLevel))
        };
    }

    private static int GetUrgencyPriority(string urgencyLevel)
    {
        return urgencyLevel.ToLower() switch
        {
            "critical" => 1,
            "high" => 2,
            "medium" => 3,
            "low" => 4,
            _ => 5
        };
    }
}
