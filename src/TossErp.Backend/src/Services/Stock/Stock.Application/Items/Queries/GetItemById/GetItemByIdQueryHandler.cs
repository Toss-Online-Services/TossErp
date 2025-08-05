using System;
using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.Items.Queries.GetItemById;

/// <summary>
/// Handler for retrieving a single item by ID
/// </summary>
public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, ItemDto?>
{
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<GetItemByIdQueryHandler> _logger;

    public GetItemByIdQueryHandler(
        IItemRepository itemRepository,
        ILogger<GetItemByIdQueryHandler> logger)
    {
        _itemRepository = itemRepository;
        _logger = logger;
    }

    public async Task<ItemDto?> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting item by ID {ItemId}", request.Id);

        var item = await _itemRepository.GetByIdAsync(request.Id, cancellationToken);

        if (item == null)
        {
            _logger.LogWarning("Item with ID {ItemId} not found", request.Id);
            return null;
        }

        _logger.LogInformation("Successfully retrieved item {ItemId}: {ItemName}", item.Id, item.ItemName);

        return new ItemDto
        {
            Id = item.Id,
            ItemCode = item.ItemCode.Value,
            ItemName = item.ItemName,
            Description = item.Description ?? string.Empty,
            ItemGroup = item.ItemGroup,
            Brand = item.Brand ?? string.Empty,
            StockUOM = item.StockUOM.Code,
            ItemType = item.ItemType.ToString(),
            ValuationMethod = item.ValuationMethod.ToString(),
            ItemStatus = item.ItemStatus.ToString(),
            PriorityLevel = item.PriorityLevel.ToString(),
            Company = item.Company,
            StandardRate = item.StandardRate,
            LastPurchaseRate = item.LastPurchaseRate,
            BaseRate = item.BaseRate,
            MinimumPrice = item.MinimumPrice,
            WeightPerUnit = item.WeightPerUnit,
            WeightUOM = item.WeightUOM,
            ReOrderLevel = item.ReOrderLevel,
            ReOrderQty = item.ReOrderQty,
            MaxQty = item.MaxQty,
            MinQty = item.MinQty,
            Length = item.Length,
            Width = item.Width,
            Height = item.Height,
            IsStockItem = item.IsStockItem,
            Disabled = item.Disabled,
            Deleted = item.Deleted,
            Created = DateTime.UtcNow,
            CreatedBy = string.Empty,
            LastModified = DateTime.UtcNow,
            LastModifiedBy = string.Empty
        };
    }
} 
