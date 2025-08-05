namespace TossErp.Stock.Application.Common.DTOs;

public class BinDto
{
    public Guid Id { get; set; }
    public string BinCode { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public bool IsActive { get; set; }
    public string? Location { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
} 
