using System;
using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Application.Common.Mappings;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Domain.Common;

namespace TossErp.Stock.Application.Items.Queries.GetItems;

/// <summary>
/// Handler for retrieving items with filtering and pagination
/// </summary>
public class GetItemsQueryHandler : IRequestHandler<GetItemsQuery, GetItemsResponse>
{
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<GetItemsQueryHandler> _logger;

    public GetItemsQueryHandler(
        IItemRepository itemRepository,
        ILogger<GetItemsQueryHandler> logger)
    {
        _itemRepository = itemRepository;
        _logger = logger;
    }

    public async Task<GetItemsResponse> Handle(GetItemsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving items with filters: {Filters}", request);

        // Parse item type if provided
        ItemType? itemType = null;
        if (!string.IsNullOrWhiteSpace(request.ItemType))
        {
            if (!Enum.TryParse<ItemType>(request.ItemType, true, out var parsedItemType))
            {
                throw new ValidationException($"Invalid ItemType: {request.ItemType}");
            }
            itemType = parsedItemType;
        }

        // Get items using repository
        var items = await _itemRepository.GetItemsAsync(
            itemCode: request.SearchTerm,
            itemName: request.SearchTerm,
            itemGroup: request.ItemGroup,
            itemType: itemType,
            isStockItem: request.IsStockItem,
            disabled: request.IsDisabled,
            page: request.Page ?? 1,
            pageSize: request.PageSize ?? 20,
            cancellationToken);

        // Get total count for pagination
        var totalCount = (int)await _itemRepository.GetCountAsync(cancellationToken);

        // Map to DTOs
        var itemDtos = items.Select(i => new ItemDto
        {
            Id = i.Id,
            ItemCode = i.ItemCode.Value,
            ItemName = i.ItemName,
            Description = i.Description ?? string.Empty,
            ItemGroup = i.ItemGroup,
            Brand = i.Brand ?? string.Empty,
            StockUOM = i.StockUOM.Code,
            ItemType = i.ItemType.ToString(),
            ValuationMethod = i.ValuationMethod.ToString(),
            StandardRate = i.StandardRate,
            MinimumPrice = i.MinimumPrice,
            IsStockItem = i.IsStockItem,
            Disabled = i.Disabled,
            Company = i.Company,
            Created = DateTime.UtcNow, // Use current time since aggregate doesn't expose Created
            CreatedBy = string.Empty, // Use empty string since aggregate doesn't expose CreatedBy
            LastModified = DateTime.UtcNow, // Use current time since aggregate doesn't expose LastModified
            LastModifiedBy = string.Empty // Use empty string since aggregate doesn't expose LastModifiedBy
        }).ToList();

        _logger.LogInformation("Retrieved {Count} items out of {TotalCount}", itemDtos.Count, totalCount);

        return new GetItemsResponse
        {
            Items = itemDtos,
            TotalCount = totalCount,
            Page = request.Page ?? 1,
            PageSize = request.PageSize ?? 20,
            TotalPages = (int)Math.Ceiling((double)totalCount / (request.PageSize ?? 20))
        };
    }
} 
