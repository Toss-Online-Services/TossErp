using System;
using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Domain.ValueObjects;
using TossErp.Stock.Domain.Exceptions;
using TossErp.Stock.Domain.Events;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;

namespace TossErp.Stock.Application.Items.Commands.CreateItem;

/// <summary>
/// Handler for creating new items in the stock system
/// </summary>
public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, ItemDto>
{
    private readonly IItemRepository _itemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateItemCommandHandler> _logger;

    public CreateItemCommandHandler(
        IItemRepository itemRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<CreateItemCommandHandler> logger)
    {
        _itemRepository = itemRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<ItemDto> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new item: {ItemCode}", request.ItemCode);

        // Validate that item code is unique
        var existingItem = await _itemRepository.GetByCodeAsync(request.ItemCode, cancellationToken);
        
        if (existingItem != null)
        {
            _logger.LogWarning("Item with code {ItemCode} already exists", request.ItemCode);
            throw new ValidationException($"Item with code '{request.ItemCode}' already exists");
        }

        // Parse enums
        if (!Enum.TryParse<ItemType>(request.ItemType, true, out var itemType))
        {
            throw new ValidationException($"Invalid ItemType: {request.ItemType}");
        }
        
        if (!Enum.TryParse<ValuationMethod>(request.ValuationMethod, true, out var valuationMethod))
        {
            throw new ValidationException($"Invalid ValuationMethod: {request.ValuationMethod}");
        }

        // Create value objects
        var itemCode = new ItemCode(request.ItemCode);
        var stockUOM = new UnitOfMeasure(request.StockUOM, request.StockUOM);
        var company = _currentUserService.CompanyId ?? "Default"; // Get from current user context, fallback to Default

        // Create the item aggregate
        var item = new ItemAggregate(
            itemCode,
            request.ItemName,
            request.ItemGroup,
            stockUOM,
            itemType,
            valuationMethod,
            company);

        // Set additional properties if provided
        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            item.UpdateBasicInfo(request.ItemName, request.Description, request.Brand, request.ItemGroup);
        }

        if (request.StandardRate.HasValue || request.MinimumPrice.HasValue)
        {
            item.UpdatePricing(request.StandardRate, request.MinimumPrice);
        }

        // Add to repository
        _itemRepository.Add(item);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created item {ItemId} with code {ItemCode}", 
            item.Id, item.ItemCode.Value);

        // Return DTO
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
            Created = DateTime.UtcNow, // Use current time since aggregate doesn't expose Created
            CreatedBy = _currentUserService.UserId ?? string.Empty,
            LastModified = DateTime.UtcNow, // Use current time since aggregate doesn't expose LastModified
            LastModifiedBy = _currentUserService.UserId ?? string.Empty
        };
    }
} 
