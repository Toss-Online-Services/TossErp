using Microsoft.Extensions.Logging;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Infrastructure.Data;

/// <summary>
/// Example usage of the Unit of Work pattern
/// This file demonstrates various ways to use the UnitOfWork for transaction management
/// </summary>
public static class UnitOfWorkUsageExample
{
    /// <summary>
    /// Example 1: Basic transaction management with manual control
    /// </summary>
    public static async Task Example1_BasicTransactionManagement(
        IUnitOfWork unitOfWork,
        IStockLevelRepository stockLevelRepository,
        IStockMovementRepository stockMovementRepository,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Begin transaction
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            // Perform multiple operations
            var stockLevel = await stockLevelRepository.GetByIdAsync(Guid.NewGuid(), cancellationToken);
            if (stockLevel != null)
            {
                // Update stock level
                stockLevel.UpdateStock(100, stockLevel.UnitCost);
                await stockLevelRepository.UpdateAsync(stockLevel, cancellationToken);

                // Create stock movement record
                var movement = StockMovement.Create(
                    Guid.NewGuid(), // tenantId
                    stockLevel.ItemId,
                    stockLevel.WarehouseId,
                    MovementType.Adjustment,
                    50,
                    "System", // createdBy
                    stockLevel.BinId,
                    stockLevel.UnitCost,
                    "ADJUSTMENT", // referenceNumber
                    "MANUAL", // referenceType
                    "Manual adjustment" // reason
                );
                await stockMovementRepository.AddAsync(movement, cancellationToken);
            }

            // Save all changes
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Commit transaction
            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            // Rollback transaction on error
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Example 2: Using the ExecuteInTransactionAsync method for automatic transaction management
    /// </summary>
    public static async Task<StockLevel> Example2_AutomaticTransactionManagement(
        IUnitOfWork unitOfWork,
        IStockLevelRepository stockLevelRepository,
        IStockMovementRepository stockMovementRepository,
        Guid itemId,
        Guid warehouseId,
        Guid binId,
        decimal quantity,
        CancellationToken cancellationToken = default)
    {
        return await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            // Get or create stock level
            var stockLevel = await stockLevelRepository.GetByItemAndWarehouseAsync(
                itemId, warehouseId, binId, cancellationToken);

            if (stockLevel == null)
            {
                stockLevel = StockLevel.Create(itemId, warehouseId, binId, quantity);
                await stockLevelRepository.AddAsync(stockLevel, cancellationToken);
            }
            else
            {
                stockLevel.ReceiveStock(quantity, stockLevel.UnitCost);
                await stockLevelRepository.UpdateAsync(stockLevel, cancellationToken);
            }

            // Create movement record
            var movement = StockMovement.CreateReceipt(
                Guid.NewGuid(), // tenantId
                itemId,
                warehouseId,
                quantity,
                "System", // createdBy
                binId,
                stockLevel.UnitCost,
                "RECEIPT", // referenceNumber
                "STOCK_RECEIPT", // referenceType
                "Stock receipt" // reason
            );
            await stockMovementRepository.AddAsync(movement, cancellationToken);

            return stockLevel;
        }, cancellationToken);
    }

    /// <summary>
    /// Example 3: Complex business operation with multiple repositories
    /// </summary>
    public static async Task Example3_ComplexBusinessOperation(
        IUnitOfWork unitOfWork,
        IItemRepository itemRepository,
        IStockLevelRepository stockLevelRepository,
        IStockMovementRepository stockMovementRepository,
        IWarehouseRepository warehouseRepository,
        CreateItemCommand command,
        CancellationToken cancellationToken = default)
    {
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            // Validate warehouse exists
            var warehouse = await warehouseRepository.GetByIdAsync(command.WarehouseId, cancellationToken);
            if (warehouse == null)
                throw new InvalidOperationException($"Warehouse with ID {command.WarehouseId} not found");

            // Create item (simplified for example)
            var itemCode = new ItemCode(command.ItemCode);
            var stockUOM = new UnitOfMeasure(command.Unit, "Stock UOM", "STOCK");
            var item = new ItemAggregate(
                itemCode,
                command.ItemName,
                command.Category,
                stockUOM,
                ItemType.Stock,
                ValuationMethod.Standard,
                "DefaultCompany"
            );
            await itemRepository.AddAsync(item, cancellationToken);

            // Create initial stock level
            var stockLevel = StockLevel.Create(
                item.Id,
                command.WarehouseId,
                command.BinId,
                command.InitialQuantity
            );
            await stockLevelRepository.AddAsync(stockLevel, cancellationToken);

            // Create initial movement record
            var movement = StockMovement.CreateReceipt(
                Guid.NewGuid(), // tenantId
                item.Id,
                command.WarehouseId,
                command.InitialQuantity,
                "System", // createdBy
                command.BinId,
                0, // unitCost
                "INITIAL", // referenceNumber
                "INITIAL_STOCK", // referenceType
                "Initial stock" // reason
            );
            await stockMovementRepository.AddAsync(movement, cancellationToken);
        }, cancellationToken);
    }

    /// <summary>
    /// Example 4: Error handling and rollback scenarios
    /// </summary>
    public static async Task Example4_ErrorHandlingAndRollback(
        IUnitOfWork unitOfWork,
        IStockLevelRepository stockLevelRepository,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Simulate some operations
                var stockLevel = await stockLevelRepository.GetByIdAsync(Guid.NewGuid(), cancellationToken);
                
                if (stockLevel == null)
                {
                    // This will cause the transaction to rollback
                    throw new InvalidOperationException("Stock level not found");
                }

                // Update stock level
                stockLevel.UpdateStock(200, stockLevel.UnitCost);
                await stockLevelRepository.UpdateAsync(stockLevel, cancellationToken);

                // Simulate another operation that might fail
                if (stockLevel.Quantity < 0)
                {
                    throw new InvalidOperationException("Quantity cannot be negative");
                }
            }, cancellationToken);

            logger.LogInformation("Transaction completed successfully");
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Transaction rolled back due to business rule violation");
            // Transaction is automatically rolled back by ExecuteInTransactionAsync
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred during transaction");
            throw;
        }
    }

    /// <summary>
    /// Example 5: Checking transaction state
    /// </summary>
    public static async Task Example5_TransactionStateChecking(
        IUnitOfWork unitOfWork,
        IStockLevelRepository stockLevelRepository,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        // Check if transaction is already active
        if (unitOfWork.HasActiveTransaction)
        {
            logger.LogWarning("Transaction is already active");
            return;
        }

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            
            // Verify transaction is now active
            if (!unitOfWork.HasActiveTransaction)
            {
                throw new InvalidOperationException("Transaction should be active");
            }

            // Perform operations
            var stockLevel = await stockLevelRepository.GetByIdAsync(Guid.NewGuid(), cancellationToken);
            if (stockLevel != null)
            {
                stockLevel.UpdateStock(150, stockLevel.UnitCost);
                await stockLevelRepository.UpdateAsync(stockLevel, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            // Verify transaction is no longer active
            if (unitOfWork.HasActiveTransaction)
            {
                throw new InvalidOperationException("Transaction should not be active after commit");
            }
        }
        catch (Exception)
        {
            if (unitOfWork.HasActiveTransaction)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
            }
            throw;
        }
    }
}

/// <summary>
/// Example command for creating an item
/// </summary>
public record CreateItemCommand(
    string ItemCode,
    string ItemName,
    string Description,
    string Category,
    string Unit,
    decimal StandardRate,
    decimal MinimumPrice,
    decimal WeightPerUnit,
    decimal Length,
    decimal Width,
    decimal Height,
    Guid WarehouseId,
    Guid BinId,
    decimal InitialQuantity
);
