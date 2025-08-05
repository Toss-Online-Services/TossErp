namespace TossErp.Stock.Application.Common.DTOs;

public class WarehouseReportDto
{
    public Guid? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public DateTime GeneratedDate { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public List<object> Data { get; set; } = new();
    public Dictionary<string, object> Summary { get; set; } = new();
} 
