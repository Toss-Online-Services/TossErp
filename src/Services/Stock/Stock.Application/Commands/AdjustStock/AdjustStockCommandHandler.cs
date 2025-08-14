using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Domain.Exceptions;

namespace TossErp.Stock.Application.Commands.AdjustStock;

/// <summary>
/// Handler for adjusting stock levels in the warehouse
/// </summary>
public class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, bool>
{
    private readonly IItemRepository _itemRepository;
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AdjustStockCommandHandler> _logger;

    public AdjustStockCommandHandler(
        IItemRepository itemRepository,
        IStockLevelRepository stockLevelRepository,
        IStockMovementRepository stockMovementRepository,
        IWarehouseRepository warehouseRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<AdjustStockCommandHandler> logger)
    {
        _itemRepository = itemRepository;
        _stockLevelRepository = stockLevelRepository;
        _stockMovementRepository = stockMovementRepository;
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<bool> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adjusting stock: ItemId={ItemId}, WarehouseId={WarehouseId}, Quantity={Quantity}",
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
            request.ItemId, request.WarehouseId, cancellationToken);

        if (stockLevel == null)
        {
            throw new InvalidStockOperationException($"No stock level found for item {request.ItemId} in warehouse {request.WarehouseId}");
        }

        // Calculate adjustment
        var oldQuantity = stockLevel.Quantity;
        var newQuantity = oldQuantity + request.Quantity;

        // Validate that adjustment won't result in negative stock
        if (newQuantity < 0)
        {
            throw new InvalidStockOperationException(
                $"Stock adjustment would result in negative stock. Current: {oldQuantity}, Adjustment: {request.Quantity}");
        }

        // Update stock level
        stockLevel.UpdateStock(newQuantity, stockLevel.UnitCost);
        _stockLevelRepository.Update(stockLevel);

        // Create stock movement record
        var stockMovement = StockMovement.CreateAdjustment(
            request.TenantId,
            request.ItemId,
            request.WarehouseId,
            request.Quantity,
            request.CreatedBy,
            request.Reason,
            null, // binId
            stockLevel.UnitCost);

        _stockMovementRepository.Add(stockMovement);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully adjusted stock: ItemId={ItemId}, WarehouseId={WarehouseId}, OldQuantity={OldQuantity}, NewQuantity={NewQuantity}",
            request.ItemId, request.WarehouseId, oldQuantity, newQuantity);

        return true;
    }
}
