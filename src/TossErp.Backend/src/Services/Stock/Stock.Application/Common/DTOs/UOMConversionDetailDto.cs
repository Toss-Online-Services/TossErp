namespace TossErp.Stock.Application.Common.DTOs;

public class UOMConversionDetailDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string UOM { get; set; } = string.Empty;
    public decimal ConversionFactor { get; set; }
    public bool IsBaseUOM { get; set; }
    public bool Disabled { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
} 
