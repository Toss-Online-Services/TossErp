using MediatR;
using TossErp.Procurement.Application.Common.DTOs;
using TossErp.Procurement.Application.Common.Interfaces;
using TossErp.Procurement.Domain.Common;
using TossErp.Procurement.Domain.Entities;
using TossErp.Procurement.Domain.Exceptions;

namespace TossErp.Procurement.Application.Commands.CreateSupplierPriceList;

/// <summary>
/// Handler for CreateSupplierPriceListCommand
/// </summary>
public class CreateSupplierPriceListCommandHandler : IRequestHandler<CreateSupplierPriceListCommand, SupplierPriceListDto>
{
    private readonly ISupplierPriceListRepository _priceListRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;

    public CreateSupplierPriceListCommandHandler(
        ISupplierPriceListRepository priceListRepository,
        ISupplierRepository supplierRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService)
    {
        _priceListRepository = priceListRepository;
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
    }

    public async Task<SupplierPriceListDto> Handle(CreateSupplierPriceListCommand request, CancellationToken cancellationToken)
    {
        // Validate supplier exists
        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier == null)
            throw new NotFoundException($"Supplier with ID {request.SupplierId} not found.");

        // Validate supplier is active
        if (supplier.Status != Domain.Enums.SupplierStatus.Active)
            throw new DomainException($"Supplier {supplier.Name} is not active");

        // Check if price list name already exists for this supplier
        var nameExists = await _priceListRepository.NameExistsForSupplierAsync(
            request.SupplierId, request.Name, null, cancellationToken);
        if (nameExists)
            throw new DomainException($"Price list with name '{request.Name}' already exists for supplier {supplier.Name}");

        // Get tenant ID from current user
        var tenantId = _currentUserService.TenantId ?? "default-tenant";
        var currentUser = _currentUserService.UserName ?? "system";

        // Create price list
        var priceList = SupplierPriceList.Create(
            request.SupplierId,
            supplier.Name,
            request.Name,
            tenantId,
            request.EffectiveFrom,
            request.Currency);

        // Set description and effective to date if provided
        if (!string.IsNullOrWhiteSpace(request.Description) || request.EffectiveTo.HasValue)
        {
            priceList.UpdateInfo(request.Name, request.Description, request.EffectiveTo, currentUser);
        }

        // Add items to price list
        foreach (var itemRequest in request.Items)
        {
            priceList.AddOrUpdateItem(
                itemRequest.ItemId,
                itemRequest.ItemName,
                itemRequest.ItemSku,
                itemRequest.UnitPrice,
                itemRequest.MinimumOrderQuantity,
                itemRequest.LeadTimeDays,
                itemRequest.Notes);
        }

        // Save price list
        await _priceListRepository.AddAsync(priceList, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(priceList.DomainEvents, cancellationToken);
        priceList.ClearDomainEvents();

        // Return DTO
        return MapToDto(priceList);
    }

    private static SupplierPriceListDto MapToDto(SupplierPriceList priceList)
    {
        return new SupplierPriceListDto
        {
            Id = priceList.Id,
            SupplierId = priceList.SupplierId,
            SupplierName = priceList.SupplierName,
            Name = priceList.Name,
            Description = priceList.Description,
            EffectiveFrom = priceList.EffectiveFrom,
            EffectiveTo = priceList.EffectiveTo,
            IsActive = priceList.IsActive,
            Currency = priceList.Currency,
            IsCurrentlyEffective = priceList.IsCurrentlyEffective,
            Items = priceList.Items.Select(MapItemToDto).ToList(),
            CreatedAt = priceList.CreatedAt,
            UpdatedAt = priceList.UpdatedAt,
            CreatedBy = priceList.CreatedBy,
            UpdatedBy = priceList.UpdatedBy,
            TenantId = priceList.TenantId
        };
    }

    private static SupplierPriceListItemDto MapItemToDto(SupplierPriceListItem item)
    {
        return new SupplierPriceListItemDto
        {
            Id = item.Id,
            SupplierPriceListId = item.SupplierPriceListId,
            ItemId = item.ItemId,
            ItemName = item.ItemName,
            ItemSku = item.ItemSku,
            UnitPrice = item.UnitPrice,
            Currency = item.Currency,
            MinimumOrderQuantity = item.MinimumOrderQuantity,
            LeadTimeDays = item.LeadTimeDays,
            Notes = item.Notes,
            LastUpdated = item.LastUpdated
        };
    }
}
