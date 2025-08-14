using System;

namespace TossErp.Stock.Application.DTOs;

public class StockLevelDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = null!;
    public string ItemCode { get; set; } = null!;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = null!;
    public Guid? BinId { get; set; }
    public string? BinCode { get; set; }
    public decimal Quantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public decimal ReorderLevel { get; set; }
    public decimal? MaxQuantity { get; set; }
    public DateTime LastMovementDate { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalValue { get; set; }
}
