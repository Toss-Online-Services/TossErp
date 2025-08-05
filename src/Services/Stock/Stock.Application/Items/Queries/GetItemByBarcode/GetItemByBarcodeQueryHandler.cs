using System;
using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace TossErp.Stock.Application.Items.Queries.GetItemByBarcode;

/// <summary>
/// Handler for retrieving an item by barcode
/// </summary>
public class GetItemByBarcodeQueryHandler : IRequestHandler<GetItemByBarcodeQuery, ItemDto?>
{
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<GetItemByBarcodeQueryHandler> _logger;

    public GetItemByBarcodeQueryHandler(
        IItemRepository itemRepository,
        ILogger<GetItemByBarcodeQueryHandler> logger)
    {
        _itemRepository = itemRepository;
        _logger = logger;
    }

    public async Task<ItemDto?> Handle(GetItemByBarcodeQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting item by barcode {Barcode}", request.Barcode);

        if (string.IsNullOrWhiteSpace(request.Barcode))
        {
            _logger.LogWarning("Barcode cannot be empty");
            return null;
        }

        // Get all items and check their barcodes
        // This is a simplified approach - in a real implementation, you'd want to optimize this
        var items = await _itemRepository.GetAllAsync(cancellationToken);
        
        var itemWithBarcode = items.FirstOrDefault(item => 
            item.Barcodes.Any(barcode => 
                barcode.Barcode.Equals(request.Barcode, StringComparison.OrdinalIgnoreCase) &&
                barcode.IsActive
            )
        );

        if (itemWithBarcode == null)
        {
            _logger.LogInformation("No item found with barcode {Barcode}", request.Barcode);
            return null;
        }

        _logger.LogInformation("Found item {ItemCode} with barcode {Barcode}", itemWithBarcode.ItemCode, request.Barcode);

        // Map to DTO
        return new ItemDto
        {
            Id = itemWithBarcode.Id,
            ItemCode = itemWithBarcode.ItemCode.Value,
            ItemName = itemWithBarcode.ItemName,
            Description = itemWithBarcode.Description ?? string.Empty,
            ItemGroup = itemWithBarcode.ItemGroup,
            Brand = itemWithBarcode.Brand ?? string.Empty,
            StockUOM = itemWithBarcode.StockUOM.Code,
            ItemType = itemWithBarcode.ItemType.ToString(),
            ValuationMethod = itemWithBarcode.ValuationMethod.ToString(),
            StandardRate = itemWithBarcode.StandardRate,
            MinimumPrice = itemWithBarcode.MinimumPrice,
            IsStockItem = itemWithBarcode.IsStockItem,
            Disabled = itemWithBarcode.Disabled,
            Company = itemWithBarcode.Company,
            Created = DateTime.UtcNow, // These would ideally come from audit properties
            CreatedBy = "System",
            LastModified = DateTime.UtcNow,
            LastModifiedBy = "System"
        };
    }
} 
