namespace TossErp.Stock.Application.Common.DTOs;

public class WarehouseAnalyticsDto
{
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    public decimal TotalValue { get; set; }
    public decimal UtilizationPercentage { get; set; }
    public int LowStockItems { get; set; }
    public int OutOfStockItems { get; set; }
    public int TotalBins { get; set; }
    public int OccupiedBins { get; set; }
    public decimal AverageItemValue { get; set; }
    public DateTime LastUpdated { get; set; }
} 
