namespace TossErp.Stock.Application.Common.DTOs;

public class ItemBarcodeDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public string BarcodeType { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public bool Disabled { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
} 
