using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Application.Common.Mappings;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Application.Warehouses.Commands.CreateWarehouse;

public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, WarehouseDto>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILogger<CreateWarehouseCommandHandler> _logger;

    public CreateWarehouseCommandHandler(
        IWarehouseRepository warehouseRepository,
        ILogger<CreateWarehouseCommandHandler> logger)
    {
        _warehouseRepository = warehouseRepository;
        _logger = logger;
    }

    public async Task<WarehouseDto> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating warehouse with code: {Code}, name: {Name}", request.Code, request.Name);

        // Create value object
        var warehouseCode = new WarehouseCode(request.Code);

        // Create the warehouse aggregate using the actual constructor
        var warehouse = new WarehouseAggregate(
            warehouseCode,
            request.Name,
            request.Company,
            request.Description
        );

        // Update additional properties if provided
        if (!string.IsNullOrEmpty(request.AddressLine1) || !string.IsNullOrEmpty(request.City) || 
            !string.IsNullOrEmpty(request.State) || !string.IsNullOrEmpty(request.Country) || 
            !string.IsNullOrEmpty(request.Pin))
        {
            warehouse.UpdateAddress(
                request.AddressLine1,
                request.AddressLine2,
                request.City,
                request.State,
                request.Pin,
                request.Country
            );
        }

        if (!string.IsNullOrEmpty(request.EmailId) || !string.IsNullOrEmpty(request.PhoneNo) || 
            !string.IsNullOrEmpty(request.MobileNo))
        {
            warehouse.UpdateContactInfo(
                request.EmailId,
                request.PhoneNo,
                request.MobileNo
            );
        }

        if (request.IsGroup)
        {
            warehouse.SetAsGroup();
        }

        if (request.IsRejectedWarehouse)
        {
            warehouse.SetAsRejectedWarehouse();
        }

        if (!string.IsNullOrEmpty(request.WarehouseType))
        {
            warehouse.UpdateWarehouseType(request.WarehouseType);
        }

        if (!string.IsNullOrEmpty(request.DefaultInTransitWarehouse))
        {
            warehouse.SetDefaultInTransitWarehouse(request.DefaultInTransitWarehouse);
        }

        if (!string.IsNullOrEmpty(request.Account))
        {
            warehouse.UpdateAccountSettings(request.Account);
        }

        if (request.IsDisabled)
        {
            warehouse.Disable();
        }

        // Save to repository
        var createdWarehouse = await _warehouseRepository.AddAsync(warehouse, cancellationToken);

        _logger.LogInformation("Warehouse created with ID: {Id}", createdWarehouse.Id);

        // Return DTO using extension method
        return createdWarehouse.ToDto();
    }
} 
