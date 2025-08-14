using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Domain.Exceptions;

namespace TossErp.Stock.Application.Commands.IssueStock;

/// <summary>
/// Handler for issuing stock from the warehouse
/// </summary>
public class IssueStockCommandHandler : IRequestHandler<IssueStockCommand, bool>
{
    private readonly IItemRepository _itemRepository;
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<IssueStockCommandHandler> _logger;

    public IssueStockCommandHandler(
        IItemRepository itemRepository,
        IStockLevelRepository stockLevelRepository,
        IStockMovementRepository stockMovementRepository,
        IWarehouseRepository warehouseRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<IssueStockCommandHandler> logger)
    {
        _itemRepository = itemRepository;
        _stockLevelRepository = stockLevelRepository;
        _stockMovementRepository = stockMovementRepository;
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<bool> Handle(IssueStockCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Issuing stock: ItemId={ItemId}, WarehouseId={WarehouseId}, Quantity={Quantity}",
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

        // Get stock level
        var stockLevel = await _stockLevelRepository.GetByItemAndWarehouseAsync(
            request.ItemId, request.WarehouseId, null, cancellationToken);

        if (stockLevel == null)
        {
            throw new InsufficientStockException($"No stock available for item {request.ItemId} in warehouse {request.WarehouseId}");
        }

        // Check if sufficient stock is available
        var availableStock = stockLevel.Quantity - stockLevel.ReservedQuantity;
        if (availableStock < request.Quantity)
        {
            throw new InsufficientStockException(
                $"Insufficient stock. Available: {availableStock}, Requested: {request.Quantity}");
        }

        // Update stock level
        stockLevel.IssueStock(request.Quantity);
        _stockLevelRepository.Update(stockLevel);

        // Create stock movement record
        var stockMovement = StockMovement.CreateIssue(
            request.TenantId,
            request.ItemId,
            request.WarehouseId,
            request.Quantity,
            request.CreatedBy,
            null, // binId
            stockLevel.UnitCost,
            request.RefId,
            request.RefType,
            request.Reason ?? "Stock issued");

        _stockMovementRepository.Add(stockMovement);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully issued stock: ItemId={ItemId}, WarehouseId={WarehouseId}, Quantity={Quantity}",
            request.ItemId, request.WarehouseId, request.Quantity);

        return true;
    }
}
