#nullable disable

using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.DTOs;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace TossErp.Stock.Application.Items.Queries.GetMovementHistory;

/// <summary>
/// Handler for retrieving stock movement history with filtering and pagination
/// </summary>
public class GetMovementHistoryQueryHandler : IRequestHandler<GetMovementHistoryQuery, GetMovementHistoryResponse>
{
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILogger<GetMovementHistoryQueryHandler> _logger;

    public GetMovementHistoryQueryHandler(
        IStockMovementRepository stockMovementRepository,
        IItemRepository itemRepository,
        IWarehouseRepository warehouseRepository,
        ILogger<GetMovementHistoryQueryHandler> logger)
    {
        _stockMovementRepository = stockMovementRepository;
        _itemRepository = itemRepository;
        _warehouseRepository = warehouseRepository;
        _logger = logger;
    }

    public async Task<GetMovementHistoryResponse> Handle(GetMovementHistoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving movement history with filters: ItemId={ItemId}, WarehouseId={WarehouseId}, MovementType={MovementType}",
            request.ItemId, request.WarehouseId, request.MovementType);

        // Build query
        var query = _stockMovementRepository.GetQueryable();

        // Apply filters
        if (request.ItemId.HasValue)
        {
            query = query.Where(sm => sm.ItemId == request.ItemId.Value);
        }

        if (request.WarehouseId.HasValue)
        {
            query = query.Where(sm => sm.WarehouseId == request.WarehouseId.Value);
        }

        if (request.BinId.HasValue)
        {
            query = query.Where(sm => sm.BinId == request.BinId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.MovementType))
        {
            if (Enum.TryParse<MovementType>(request.MovementType, true, out var movementType))
            {
                query = query.Where(sm => sm.MovementType == movementType);
            }
        }

        if (request.BatchId.HasValue)
        {
            query = query.Where(sm => sm.BatchId == request.BatchId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.ReferenceNumber))
        {
            query = query.Where(sm => sm.ReferenceNumber.Contains(request.ReferenceNumber));
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(sm => sm.MovementDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(sm => sm.MovementDate <= request.ToDate.Value);
        }

        // Apply sorting
        query = ApplySorting(query, request.SortBy ?? string.Empty, request.SortOrder ?? string.Empty);

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var page = request.Page ?? 1;
        var pageSize = request.PageSize ?? 20;
        var skip = (page - 1) * pageSize;

        var movements = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        // Convert to DTOs
        var movementDtos = movements.Select(sm => new MovementDto
        {
            Id = sm.Id,
            ItemId = sm.ItemId,
            ItemName = sm.Item != null ? sm.Item.ItemName : string.Empty,
            ItemCode = sm.Item != null && sm.Item.ItemCode != null ? sm.Item.ItemCode.Value : string.Empty,
            WarehouseId = sm.WarehouseId,
            WarehouseName = sm.Warehouse != null ? sm.Warehouse.Name : string.Empty,
            BinId = sm.BinId,
            BinCode = sm.Bin != null && sm.Bin.BinCode != null ? sm.Bin.BinCode.Value : null,
            MovementType = sm.MovementType.ToString(),
            Quantity = sm.Quantity,
            UnitCost = sm.UnitCost ?? 0,
            TotalCost = sm.Quantity * (sm.UnitCost ?? 0),
            ReferenceNumber = sm.ReferenceNumber,
            Reason = sm.Reason,
            MovementDate = sm.MovementDate,
            BatchId = sm.BatchId,
            BatchNumber = sm.Batch != null ? sm.Batch.Name : null,
            CreatedBy = sm.CreatedBy,
            CreatedAt = sm.CreatedAt
        }).ToList();

        // Calculate summary statistics
        var summary = new MovementHistorySummary
        {
            TotalReceived = movements.Where(sm => sm.MovementType == MovementType.Receipt).Sum(sm => sm.Quantity),
            TotalIssued = movements.Where(sm => sm.MovementType == MovementType.Issue).Sum(sm => sm.Quantity),
            TotalAdjusted = movements.Where(sm => sm.MovementType == MovementType.Adjustment).Sum(sm => sm.Quantity),
            TotalTransferred = movements.Where(sm => sm.MovementType == MovementType.Transfer).Sum(sm => sm.Quantity),
            NetMovement = movements.Sum(sm => sm.Quantity * (sm.MovementType == MovementType.Issue ? -1 : 1))
        };

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        _logger.LogInformation("Retrieved {Count} movements out of {TotalCount} total", 
            movementDtos.Count, totalCount);

        return new GetMovementHistoryResponse
        {
            Movements = movementDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            Summary = summary
        };
    }

    private static IQueryable<StockMovement> ApplySorting(IQueryable<StockMovement> query, string sortBy, string sortOrder)
    {
        var isDescending = sortOrder != null && sortOrder.ToLower() == "desc";

        return sortBy != null ? sortBy.ToLower() switch
        {
            "date" => isDescending ? query.OrderByDescending(sm => sm.MovementDate) : query.OrderBy(sm => sm.MovementDate),
            "itemname" => isDescending ? query.OrderByDescending(sm => sm.Item.ItemName) : query.OrderBy(sm => sm.Item.ItemName),
            "warehousename" => isDescending ? query.OrderByDescending(sm => sm.Warehouse.Name) : query.OrderBy(sm => sm.Warehouse.Name),
            "quantity" => isDescending ? query.OrderByDescending(sm => sm.Quantity) : query.OrderBy(sm => sm.Quantity),
            "movementtype" => isDescending ? query.OrderByDescending(sm => sm.MovementType) : query.OrderBy(sm => sm.MovementType),
            _ => isDescending ? query.OrderByDescending(sm => sm.MovementDate) : query.OrderBy(sm => sm.MovementDate)
        } : isDescending ? query.OrderByDescending(sm => sm.MovementDate) : query.OrderBy(sm => sm.MovementDate);
    }
}
