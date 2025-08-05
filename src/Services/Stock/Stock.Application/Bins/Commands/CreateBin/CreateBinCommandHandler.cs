using MediatR;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate.Entities;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.ValueObjects;
using TossErp.Stock.Domain.Exceptions;

namespace TossErp.Stock.Application.Bins.Commands.CreateBin;

public class CreateBinCommandHandler : IRequestHandler<CreateBinCommand, BinDto>
{
    private readonly IBinRepository _binRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBinCommandHandler(
        IBinRepository binRepository,
        IWarehouseRepository warehouseRepository,
        IItemRepository itemRepository,
        IUnitOfWork unitOfWork)
    {
        _binRepository = binRepository;
        _warehouseRepository = warehouseRepository;
        _itemRepository = itemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BinDto> Handle(CreateBinCommand request, CancellationToken cancellationToken)
    {
        // Get the warehouse
        var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId, cancellationToken);
        if (warehouse == null)
        {
            throw new TossErp.Stock.Domain.Exceptions.NotFoundException($"Warehouse with ID {request.WarehouseId} not found.");
        }

        // Create value objects
        var binCode = new BinCode(request.BinCode);

        // Create the bin using the correct constructor
        var bin = new Bin(
            binCode,
            request.BinName ?? request.BinCode, // Use BinName or fallback to BinCode
            request.Description
        );

        // Save to repository
        await _binRepository.AddAsync(bin, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return DTO
        return new BinDto
        {
            Id = bin.Id,
            BinCode = bin.BinCode.Value,
            WarehouseCode = string.Empty, // Not available in Bin entity
            ItemCode = string.Empty, // Not available in Bin entity
            Quantity = 0, // Not available in Bin entity
            IsActive = bin.IsActive,
            Location = bin.Description ?? string.Empty, // Use Description as Location
            Created = DateTime.UtcNow, // Use current time since entity doesn't expose Created
            CreatedBy = string.Empty, // Use empty string since entity doesn't expose CreatedBy
            LastModified = DateTime.UtcNow, // Use current time since entity doesn't expose LastModified
            LastModifiedBy = string.Empty // Use empty string since entity doesn't expose LastModifiedBy
        };
    }
} 
