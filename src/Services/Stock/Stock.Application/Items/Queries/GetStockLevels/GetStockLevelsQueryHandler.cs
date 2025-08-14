using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.DTOs;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TossErp.Stock.Application.Items.Queries.GetStockLevels;

/// <summary>
/// Handler for retrieving stock levels with filtering and pagination
/// </summary>
public class GetStockLevelsQueryHandler : IRequestHandler<GetStockLevelsQuery, GetStockLevelsResponse>
{
    private readonly IItemRepository _itemRepository;
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILogger<GetStockLevelsQueryHandler> _logger;

    public GetStockLevelsQueryHandler(
        IItemRepository itemRepository,
        IStockLevelRepository stockLevelRepository,
        IWarehouseRepository warehouseRepository,
        ILogger<GetStockLevelsQueryHandler> logger)
    {
        _itemRepository = itemRepository;
        _stockLevelRepository = stockLevelRepository;
        _warehouseRepository = warehouseRepository;
        _logger = logger;
    }

    public async Task<GetStockLevelsResponse> Handle(GetStockLevelsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving stock levels with filters: ItemId={ItemId}, WarehouseId={WarehouseId}, LowStockOnly={LowStockOnly}",
            request.ItemId, request.WarehouseId, request.LowStockOnly);

        // Build query
        var query = _stockLevelRepository.GetQueryable();

        // Apply filters
        if (request.ItemId.HasValue)
        {
            query = query.Where(sl => sl.ItemId == request.ItemId.Value);
        }

        if (request.WarehouseId.HasValue)
        {
            query = query.Where(sl => sl.WarehouseId == request.WarehouseId.Value);
        }

        if (request.BinId.HasValue)
        {
            query = query.Where(sl => sl.BinId == request.BinId.Value);
        }

        if (request.LowStockOnly == true)
        {
            query = query.Where(sl => sl.Item.ReOrderLevel.HasValue && sl.Quantity <= sl.Item.ReOrderLevel.Value);
        }

        if (request.OutOfStockOnly == true)
        {
            query = query.Where(sl => sl.Quantity <= 0);
        }

        if (request.InStockOnly == true)
        {
            query = query.Where(sl => sl.Quantity > 0);
        }

        // Apply sorting
        query = ApplySorting(query, request.SortBy, request.SortOrder);

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var page = request.Page ?? 1;
        var pageSize = request.PageSize ?? 20;
        var skip = (page - 1) * pageSize;

        var stockLevels = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        // Convert to DTOs
        var stockLevelDtos = stockLevels.Select(sl => new StockLevelDto
        {
            Id = sl.Id,
            ItemId = sl.ItemId,
            ItemName = sl.Item.ItemName,
            ItemCode = sl.Item.ItemCode.Value,
            WarehouseId = sl.WarehouseId,
            WarehouseName = sl.Warehouse.Name,
            BinId = sl.BinId,
            BinCode = sl.Bin?.BinCode.Value,
            Quantity = sl.Quantity,
            ReservedQuantity = sl.ReservedQuantity,
            AvailableQuantity = sl.Quantity - sl.ReservedQuantity,
            ReorderLevel = sl.Item.ReOrderLevel ?? 0m,
            MaxQuantity = sl.Item.MaxQty ?? 0m,
            LastMovementDate = sl.LastMovementDate,
            UnitCost = sl.UnitCost,
            TotalValue = sl.Quantity * sl.UnitCost
        }).ToList();

        // Calculate summary statistics
        var summary = new StockLevelSummary
        {
            TotalItemsInStock = stockLevels.Count(sl => sl.Quantity > 0),
            TotalItemsOutOfStock = stockLevels.Count(sl => sl.Quantity <= 0),
            TotalItemsLowStock = stockLevels.Count(sl => sl.Item.ReOrderLevel.HasValue && sl.Quantity <= sl.Item.ReOrderLevel.Value),
            TotalStockValue = stockLevels.Sum(sl => sl.Quantity * sl.UnitCost)
        };

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        _logger.LogInformation("Retrieved {Count} stock levels out of {TotalCount} total", 
            stockLevelDtos.Count, totalCount);

        return new GetStockLevelsResponse
        {
            StockLevels = stockLevelDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            Summary = summary
        };
    }

    private static IQueryable<StockLevel> ApplySorting(IQueryable<StockLevel> query, string? sortBy, string? sortOrder)
    {
        var isDescending = sortOrder?.ToLower() == "desc";

        return sortBy?.ToLower() switch
        {
            "itemname" => isDescending ? query.OrderByDescending(sl => sl.Item.ItemName) : query.OrderBy(sl => sl.Item.ItemName),
            "warehousename" => isDescending ? query.OrderByDescending(sl => sl.Warehouse.Name) : query.OrderBy(sl => sl.Warehouse.Name),
            "quantity" => isDescending ? query.OrderByDescending(sl => sl.Quantity) : query.OrderBy(sl => sl.Quantity),
            "reorderlevel" => isDescending ? query.OrderByDescending(sl => sl.Item.ReOrderLevel) : query.OrderBy(sl => sl.Item.ReOrderLevel),
            _ => isDescending ? query.OrderByDescending(sl => sl.Item.ItemName) : query.OrderBy(sl => sl.Item.ItemName)
        };
    }
}
