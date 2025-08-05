using System;
using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.Items.Queries.GetItemBySku;

/// <summary>
/// Handler for retrieving an item by SKU
/// </summary>
public class GetItemBySkuQueryHandler : IRequestHandler<GetItemBySkuQuery, ItemDto?>
{
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<GetItemBySkuQueryHandler> _logger;

    public GetItemBySkuQueryHandler(
        IItemRepository itemRepository,
        ILogger<GetItemBySkuQueryHandler> logger)
    {
        _itemRepository = itemRepository;
        _logger = logger;
    }

    public async Task<ItemDto?> Handle(GetItemBySkuQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting item by SKU {Sku}", request.Sku);

        var item = await _itemRepository.GetByCodeAsync(request.Sku, cancellationToken);

        if (item == null)
        {
            _logger.LogWarning("Item with SKU {Sku} not found", request.Sku);
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
            StandardRate = item.StandardRate,
            MinimumPrice = item.MinimumPrice,
            IsStockItem = item.IsStockItem,
            Disabled = item.Disabled,
            Company = item.Company,
            Created = DateTime.UtcNow,
            CreatedBy = string.Empty,
            LastModified = DateTime.UtcNow,
            LastModifiedBy = string.Empty
        };
    }
} 
