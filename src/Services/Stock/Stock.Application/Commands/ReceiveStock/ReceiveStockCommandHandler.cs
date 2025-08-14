using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Domain.Exceptions;
using TossErp.Stock.Domain.Events;

namespace TossErp.Stock.Application.Commands.ReceiveStock;

/// <summary>
/// Handler for receiving stock into the warehouse
/// </summary>
public class ReceiveStockCommandHandler : IRequestHandler<ReceiveStockCommand, bool>
{
    private readonly IItemRepository _itemRepository;
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ReceiveStockCommandHandler> _logger;

    public ReceiveStockCommandHandler(
        IItemRepository itemRepository,
        IStockLevelRepository stockLevelRepository,
        IStockMovementRepository stockMovementRepository,
        IWarehouseRepository warehouseRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<ReceiveStockCommandHandler> logger)
    {
        _itemRepository = itemRepository;
        _stockLevelRepository = stockLevelRepository;
        _stockMovementRepository = stockMovementRepository;
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<bool> Handle(ReceiveStockCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Receiving stock: ItemId={ItemId}, WarehouseId={WarehouseId}, Quantity={Quantity}",
            request.ItemId, request.WarehouseId, request.Quantity);

        // Validate item exists
        var item = await _itemRepository.GetByIdAsync(request.ItemId, cancellationToken);
        if (item == null)
        {
            throw new ItemNotFoundException($"Item with ID {request.ItemId} not found");
        }

        // Validate warehouse exists
        var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId, cancellationToken);
        if (warehouse == null)
        {
            throw new WarehouseNotFoundException($"Warehouse with ID {request.WarehouseId} not found");
        }

        // Get or create stock level
        var stockLevel = await _stockLevelRepository.GetByItemAndWarehouseAsync(
            request.ItemId, request.WarehouseId, cancellationToken);

        if (stockLevel == null)
        {
            // Create new stock level
            stockLevel = StockLevel.Create(
                request.ItemId, 
                request.WarehouseId, 
                null, // binId
                request.Quantity, 
                request.UnitCost ?? 0);
            _stockLevelRepository.Add(stockLevel);
        }
        else
        {
            // Update existing stock level
            stockLevel.ReceiveStock(request.Quantity, request.UnitCost ?? 0);
            _stockLevelRepository.Update(stockLevel);
        }

        // Create stock movement record
        var stockMovement = StockMovement.CreateReceipt(
            request.TenantId,
            request.ItemId,
            request.WarehouseId,
            request.Quantity,
            request.CreatedBy,
            null, // binId
            request.UnitCost,
            request.RefId,
            request.RefType,
            request.Reason ?? "Stock received");

        _stockMovementRepository.Add(stockMovement);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully received stock: ItemId={ItemId}, WarehouseId={WarehouseId}, Quantity={Quantity}",
            request.ItemId, request.WarehouseId, request.Quantity);

        return true;
    }
}
