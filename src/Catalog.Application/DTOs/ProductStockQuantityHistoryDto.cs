namespace Catalog.Application.DTOs;

public class ProductStockQuantityHistoryDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int QuantityAdjustment { get; set; }
    public int StockQuantity { get; set; }
    public string Message { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public int? OrderId { get; set; }
} 
