using System;
using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using Microsoft.Extensions.Logging;

namespace TossErp.Stock.Application.Items.Commands.UpdateItem;

/// <summary>
/// Handler for updating items
/// </summary>
public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, ItemDto>
{
    private readonly IItemRepository _itemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateItemCommandHandler> _logger;

    public UpdateItemCommandHandler(
        IItemRepository itemRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<UpdateItemCommandHandler> logger)
    {
        _itemRepository = itemRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<ItemDto> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating item {ItemId}", request.Id);

        var item = await _itemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (item == null)
        {
            throw new TossErp.Stock.Domain.Exceptions.NotFoundException($"Item with ID {request.Id} not found");
        }

        // Update basic information
        if (!string.IsNullOrWhiteSpace(request.ItemName) || request.Description != null || request.Brand != null || request.ItemGroup != null)
        {
            item.UpdateBasicInfo(
                request.ItemName ?? item.ItemName,
                request.Description ?? item.Description,
                request.Brand ?? item.Brand,
                request.ItemGroup ?? item.ItemGroup
            );
        }

        // Update pricing
        if (request.StandardRate.HasValue || request.MinimumPrice.HasValue)
        {
            item.UpdatePricing(
                request.StandardRate ?? item.StandardRate,
                request.MinimumPrice ?? item.MinimumPrice
            );
        }

        // Update valuation method
        if (!string.IsNullOrWhiteSpace(request.ValuationMethod))
        {
            if (Enum.TryParse<TossErp.Stock.Domain.Enums.ValuationMethod>(request.ValuationMethod, true, out var valuationMethod))
            {
                item.UpdateValuationMethod(valuationMethod);
            }
            else
            {
                _logger.LogWarning("Invalid valuation method {ValuationMethod} for item {ItemId}", request.ValuationMethod, request.Id);
            }
        }

        // Update disabled status
        if (request.Disabled.HasValue)
        {
            if (request.Disabled.Value && !item.Disabled)
            {
                item.Disable();
            }
            else if (!request.Disabled.Value && item.Disabled)
            {
                item.Enable();
            }
        }

        // Update the item in the repository
        _itemRepository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully updated item {ItemId}", request.Id);

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
            CreatedBy = _currentUserService.UserId ?? string.Empty,
            LastModified = DateTime.UtcNow,
            LastModifiedBy = _currentUserService.UserId ?? string.Empty
        };
    }
}
