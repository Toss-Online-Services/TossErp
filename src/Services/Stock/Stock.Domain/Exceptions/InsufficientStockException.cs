namespace TossErp.Stock.Domain.Exceptions;

public class InsufficientStockException : StockDomainException
{
    public Guid ItemId { get; }
    public Guid WarehouseId { get; }
    public decimal RequestedQuantity { get; }
    public decimal AvailableQuantity { get; }

    public InsufficientStockException(Guid itemId, Guid warehouseId, decimal requestedQuantity, decimal availableQuantity)
        : base($"Insufficient stock for item {itemId} in warehouse {warehouseId}. Requested: {requestedQuantity}, Available: {availableQuantity}")
    {
        ItemId = itemId;
        WarehouseId = warehouseId;
        RequestedQuantity = requestedQuantity;
        AvailableQuantity = availableQuantity;
    }

    public InsufficientStockException(string message)
        : base(message)
    {
    }

    public InsufficientStockException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
} 
