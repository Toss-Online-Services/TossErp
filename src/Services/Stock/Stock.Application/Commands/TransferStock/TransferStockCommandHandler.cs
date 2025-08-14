using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Domain.Exceptions;

namespace TossErp.Stock.Application.Commands.TransferStock;

/// <summary>
/// Handler for transferring stock between warehouses
/// </summary>
public class TransferStockCommandHandler : IRequestHandler<TransferStockCommand, bool>
{
    private readonly IItemRepository _itemRepository;
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<TransferStockCommandHandler> _logger;

    public TransferStockCommandHandler(
        IItemRepository itemRepository,
        IStockLevelRepository stockLevelRepository,
        IStockMovementRepository stockMovementRepository,
        IWarehouseRepository warehouseRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<TransferStockCommandHandler> logger)
    {
        _itemRepository = itemRepository;
        _stockLevelRepository = stockLevelRepository;
        _stockMovementRepository = stockMovementRepository;
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<bool> Handle(TransferStockCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Transferring stock: ItemId={ItemId}, FromWarehouseId={FromWarehouseId}, ToWarehouseId={ToWarehouseId}, Quantity={Quantity}",
            request.ItemId, request.FromWarehouseId, request.ToWarehouseId, request.Quantity);

        // Validate item exists
        var item = await _itemRepository.GetByIdAsync(request.ItemId, cancellationToken);
        if (item == null)
        {
            throw new ItemNotFoundException($"Item with ID {request.ItemId} not found");
        }

        // Validate source warehouse exists
        var fromWarehouse = await _warehouseRepository.GetByIdAsync(request.FromWarehouseId, cancellationToken);
        if (fromWarehouse == null)
        {
            throw new WarehouseNotFoundException($"Source warehouse with ID {request.FromWarehouseId} not found");
        }

        // Validate destination warehouse exists
        var toWarehouse = await _warehouseRepository.GetByIdAsync(request.ToWarehouseId, cancellationToken);
        if (toWarehouse == null)
        {
            throw new WarehouseNotFoundException($"Destination warehouse with ID {request.ToWarehouseId} not found");
        }

        // Get source stock level
        var fromStockLevel = await _stockLevelRepository.GetByItemAndWarehouseAsync(
            request.ItemId, request.FromWarehouseId, cancellationToken);

        if (fromStockLevel == null)
        {
            throw new InsufficientStockException($"No stock available for item {request.ItemId} in source warehouse {request.FromWarehouseId}");
        }

        // Check if sufficient stock is available in source warehouse
        var availableStock = fromStockLevel.Quantity - fromStockLevel.ReservedQuantity;
        if (availableStock < request.Quantity)
        {
            throw new InsufficientStockException(
                $"Insufficient stock in source warehouse. Available: {availableStock}, Requested: {request.Quantity}");
        }

        // Get or create destination stock level
        var toStockLevel = await _stockLevelRepository.GetByItemAndWarehouseAsync(
            request.ItemId, request.ToWarehouseId, cancellationToken);

        if (toStockLevel == null)
        {
            // Create new stock level in destination warehouse
            toStockLevel = StockLevel.Create(
                request.ItemId, 
                request.ToWarehouseId, 
                null, // binId
                request.Quantity, 
                fromStockLevel.UnitCost);
            _stockLevelRepository.Add(toStockLevel);
        }
        else
        {
            // Update existing stock level in destination warehouse
            toStockLevel.ReceiveStock(request.Quantity, fromStockLevel.UnitCost);
            _stockLevelRepository.Update(toStockLevel);
        }

        // Update source stock level
        fromStockLevel.IssueStock(request.Quantity);
        _stockLevelRepository.Update(fromStockLevel);

        // Create stock movement records (out from source, in to destination)
        var outMovement = StockMovement.CreateTransfer(
            request.TenantId,
            request.ItemId,
            request.FromWarehouseId,
            request.Quantity,
            request.CreatedBy,
            $"Transfer to {toWarehouse.Name}: {request.Reason}",
            null, // binId
            fromStockLevel.UnitCost);

        var inMovement = StockMovement.CreateTransfer(
            request.TenantId,
            request.ItemId,
            request.ToWarehouseId,
            request.Quantity,
            request.CreatedBy,
            $"Transfer from {fromWarehouse.Name}: {request.Reason}",
            null, // binId
            fromStockLevel.UnitCost);

        _stockMovementRepository.Add(outMovement);
        _stockMovementRepository.Add(inMovement);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully transferred stock: ItemId={ItemId}, FromWarehouseId={FromWarehouseId}, ToWarehouseId={ToWarehouseId}, Quantity={Quantity}",
            request.ItemId, request.FromWarehouseId, request.ToWarehouseId, request.Quantity);

        return true;
    }
}
