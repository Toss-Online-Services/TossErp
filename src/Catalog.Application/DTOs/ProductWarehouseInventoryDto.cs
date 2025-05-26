namespace Catalog.Application.DTOs;

public class ProductWarehouseInventoryDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; }
    public int StockQuantity { get; set; }
    public int ReservedQuantity { get; set; }
} 
