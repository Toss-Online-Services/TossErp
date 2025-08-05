namespace TossErp.Stock.Application.Common.DTOs;

public class ItemReorderDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string Warehouse { get; set; } = string.Empty;
    public decimal ReOrderLevel { get; set; }
    public decimal ReOrderQty { get; set; }
    public decimal MinQty { get; set; }
    public decimal MaxQty { get; set; }
    public bool Disabled { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
} 
