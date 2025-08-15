# Unit of Work Pattern Implementation

## Overview

The Unit of Work pattern is a design pattern that maintains a list of objects affected by a business transaction and coordinates the writing out of changes and the resolution of concurrency problems. In the context of Entity Framework Core, it provides a way to manage database transactions and ensure data consistency across multiple repository operations.

## Features

### Core Functionality
- **Transaction Management**: Begin, commit, and rollback database transactions
- **Change Tracking**: Automatic tracking of all changes made through repositories
- **Atomic Operations**: Ensure all operations within a transaction succeed or fail together
- **Resource Management**: Proper disposal of database connections and transactions

### Enhanced Features
- **Transaction State Tracking**: Check if a transaction is currently active
- **Automatic Transaction Scope**: Execute operations within an automatic transaction scope
- **Comprehensive Logging**: Detailed logging for debugging and monitoring
- **Error Handling**: Robust error handling with automatic rollback on exceptions
- **Resource Cleanup**: Proper disposal pattern implementation

## Interface

```csharp
public interface IUnitOfWork : IDisposable
{
    bool HasActiveTransaction { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default);
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);
}
```

## Usage Patterns

### 1. Manual Transaction Management

Use this pattern when you need fine-grained control over transaction boundaries:

```csharp
public async Task ManualTransactionExample(IUnitOfWork unitOfWork)
{
    try
    {
        // Begin transaction
        await unitOfWork.BeginTransactionAsync();

        // Perform multiple operations
        await repository1.AddAsync(entity1);
        await repository2.UpdateAsync(entity2);
        await repository3.DeleteAsync(entity3);

        // Save all changes
        await unitOfWork.SaveChangesAsync();

        // Commit transaction
        await unitOfWork.CommitTransactionAsync();
    }
    catch (Exception)
    {
        // Rollback transaction on error
        await unitOfWork.RollbackTransactionAsync();
        throw;
    }
}
```

### 2. Automatic Transaction Scope

Use this pattern for simpler, more concise code with automatic transaction management:

```csharp
public async Task<StockLevel> AutomaticTransactionExample(IUnitOfWork unitOfWork)
{
    return await unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        // All operations within this scope are part of a single transaction
        var stockLevel = await stockLevelRepository.GetByIdAsync(id);
        stockLevel.UpdateQuantity(100);
        await stockLevelRepository.UpdateAsync(stockLevel);

        var movement = StockMovement.Create(/* parameters */);
        await movementRepository.AddAsync(movement);

        return stockLevel;
    });
}
```

### 3. Complex Business Operations

For complex operations involving multiple repositories:

```csharp
public async Task ComplexBusinessOperation(IUnitOfWork unitOfWork, CreateItemCommand command)
{
    await unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        // Validate prerequisites
        var warehouse = await warehouseRepository.GetByIdAsync(command.WarehouseId);
        if (warehouse == null)
            throw new InvalidOperationException("Warehouse not found");

        // Create item
        var item = Item.Create(/* parameters */);
        await itemRepository.AddAsync(item);

        // Create stock level
        var stockLevel = StockLevel.Create(item.Id, command.WarehouseId, command.BinId, command.InitialQuantity);
        await stockLevelRepository.AddAsync(stockLevel);

        // Create movement record
        var movement = StockMovement.Create(/* parameters */);
        await movementRepository.AddAsync(movement);
    });
}
```

## Error Handling

### Automatic Rollback
The `ExecuteInTransactionAsync` method automatically rolls back the transaction if an exception occurs:

```csharp
try
{
    await unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        // If any operation throws an exception, the entire transaction is rolled back
        await repository1.AddAsync(entity1);
        throw new InvalidOperationException("Something went wrong");
        await repository2.UpdateAsync(entity2); // This won't execute
    });
}
catch (InvalidOperationException ex)
{
    // Transaction is automatically rolled back
    logger.LogWarning("Transaction rolled back: {Message}", ex.Message);
}
```

### Manual Error Handling
For manual transaction management, always ensure rollback in catch blocks:

```csharp
try
{
    await unitOfWork.BeginTransactionAsync();
    // ... operations ...
    await unitOfWork.CommitTransactionAsync();
}
catch (Exception)
{
    await unitOfWork.RollbackTransactionAsync();
    throw;
}
```

## Best Practices

### 1. Use ExecuteInTransactionAsync for Simple Cases
Prefer the automatic transaction scope for most use cases as it's safer and more concise.

### 2. Check Transaction State
Use `HasActiveTransaction` to check if a transaction is already active:

```csharp
if (unitOfWork.HasActiveTransaction)
{
    // Handle nested transaction scenario
    return;
}
```

### 3. Proper Resource Disposal
The UnitOfWork implements `IDisposable` and should be used with `using` statements:

```csharp
using var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
await unitOfWork.ExecuteInTransactionAsync(async () =>
{
    // Operations
});
```

### 4. Cancellation Token Support
Always pass cancellation tokens for long-running operations:

```csharp
await unitOfWork.ExecuteInTransactionAsync(async () =>
{
    // Operations
}, cancellationToken);
```

### 5. Logging and Monitoring
The UnitOfWork provides comprehensive logging for monitoring:

```json
{
  "LogLevel": {
    "TossErp.Stock.Infrastructure.Data.UnitOfWork": "Debug"
  }
}
```

## Integration with Dependency Injection

The UnitOfWork is registered in the DI container:

```csharp
// In DependencyInjection.cs
services.AddScoped<IUnitOfWork, UnitOfWork>();
```

### Usage in Services

```csharp
public class StockService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStockLevelRepository _stockLevelRepository;

    public StockService(IUnitOfWork unitOfWork, IStockLevelRepository stockLevelRepository)
    {
        _unitOfWork = unitOfWork;
        _stockLevelRepository = stockLevelRepository;
    }

    public async Task UpdateStockLevelAsync(Guid itemId, decimal quantity)
    {
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var stockLevel = await _stockLevelRepository.GetByIdAsync(itemId);
            stockLevel.UpdateQuantity(quantity);
            await _stockLevelRepository.UpdateAsync(stockLevel);
        });
    }
}
```

## Transaction Isolation

The UnitOfWork uses the default transaction isolation level (ReadCommitted). For specific isolation levels, you can extend the implementation:

```csharp
public async Task BeginTransactionWithIsolationAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
{
    _transaction = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
}
```

## Performance Considerations

### 1. Transaction Duration
Keep transactions as short as possible to minimize lock contention.

### 2. Batch Operations
For large operations, consider batching:

```csharp
await unitOfWork.ExecuteInTransactionAsync(async () =>
{
    foreach (var batch in items.Chunk(1000))
    {
        foreach (var item in batch)
        {
            await repository.AddAsync(item);
        }
        await unitOfWork.SaveChangesAsync(); // Save in batches
    }
});
```

### 3. Connection Management
The UnitOfWork properly manages database connections and transactions, ensuring efficient resource usage.

## Testing

### Unit Testing
Mock the IUnitOfWork interface for unit tests:

```csharp
[Fact]
public async Task UpdateStockLevel_ShouldCommitTransaction()
{
    // Arrange
    var mockUnitOfWork = new Mock<IUnitOfWork>();
    var service = new StockService(mockUnitOfWork.Object, mockRepository.Object);

    // Act
    await service.UpdateStockLevelAsync(itemId, quantity);

    // Assert
    mockUnitOfWork.Verify(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()), Times.Once);
}
```

### Integration Testing
Use the real UnitOfWork with an in-memory database:

```csharp
[Fact]
public async Task UnitOfWork_ShouldRollbackOnException()
{
    // Arrange
    var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
    var repository = serviceProvider.GetRequiredService<IStockLevelRepository>();

    // Act & Assert
    await Assert.ThrowsAsync<InvalidOperationException>(async () =>
    {
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await repository.AddAsync(entity);
            throw new InvalidOperationException("Test exception");
        });
    });

    // Verify rollback occurred
    var savedEntity = await repository.GetByIdAsync(entity.Id);
    Assert.Null(savedEntity);
}
```

## Troubleshooting

### Common Issues

1. **"A transaction is already active"**
   - Check if you're trying to begin a transaction when one is already active
   - Use `HasActiveTransaction` to check transaction state

2. **"No active transaction to commit"**
   - Ensure you've called `BeginTransactionAsync` before `CommitTransactionAsync`
   - Check if the transaction was already committed or rolled back

3. **Memory Leaks**
   - Always use `using` statements or ensure proper disposal
   - The UnitOfWork implements `IDisposable` for automatic cleanup

4. **Deadlocks**
   - Keep transactions short
   - Access tables in a consistent order
   - Consider using appropriate isolation levels

### Debugging

Enable debug logging to trace transaction operations:

```json
{
  "LogLevel": {
    "TossErp.Stock.Infrastructure.Data.UnitOfWork": "Debug"
  }
}
```

## Conclusion

The Unit of Work pattern provides a robust foundation for managing database transactions in the Stock module. It ensures data consistency, provides comprehensive error handling, and offers both manual and automatic transaction management options. By following the best practices outlined in this documentation, you can build reliable and maintainable data access code.
