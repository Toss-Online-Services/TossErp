namespace TossErp.Stock.Application.Common.DTOs;

public class ItemPriceDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string PriceList { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal? MinimumQty { get; set; }
    public decimal? MaximumQty { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUpto { get; set; }
    public bool Disabled { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
} 
