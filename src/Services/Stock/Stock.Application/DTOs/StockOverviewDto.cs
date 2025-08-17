namespace TossErp.Stock.Application.DTOs;

public class StockOverviewDto
{
    public int TotalItems { get; set; }
    public int LowStockItems { get; set; }
    public int OutOfStockItems { get; set; }
    public decimal TotalValue { get; set; }
    public int TotalCategories { get; set; }
    public List<CategorySummaryDto> CategorySummary { get; set; } = new();
}

public class CategorySummaryDto
{
    public string Category { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public decimal TotalValue { get; set; }
}

