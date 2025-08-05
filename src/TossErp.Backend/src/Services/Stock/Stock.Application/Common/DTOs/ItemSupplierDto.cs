namespace TossErp.Stock.Application.Common.DTOs;

public class ItemSupplierDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public string SupplierPartNo { get; set; } = string.Empty;
    public decimal? LastPurchaseRate { get; set; }
    public decimal? MinimumQty { get; set; }
    public decimal? MaximumQty { get; set; }
    public decimal? LeadTimeDays { get; set; }
    public bool IsDefaultSupplier { get; set; }
    public bool Disabled { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
} 
