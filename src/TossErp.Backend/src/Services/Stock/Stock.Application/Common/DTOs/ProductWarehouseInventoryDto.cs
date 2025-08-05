namespace TossErp.Stock.Application.Common.DTOs;

public class ProductWarehouseInventoryDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public decimal StockQuantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
} 
