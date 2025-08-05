using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Domain.Exceptions;

public class WarehouseNotFoundException : StockDomainException
{
    public WarehouseCode WarehouseCode { get; }

    public WarehouseNotFoundException(WarehouseCode warehouseCode) 
        : base($"Warehouse with code '{warehouseCode}' was not found.")
    {
        WarehouseCode = warehouseCode;
    }

    public WarehouseNotFoundException(string warehouseCode) 
        : base($"Warehouse with code '{warehouseCode}' was not found.")
    {
        WarehouseCode = new WarehouseCode(warehouseCode);
    }
} 
