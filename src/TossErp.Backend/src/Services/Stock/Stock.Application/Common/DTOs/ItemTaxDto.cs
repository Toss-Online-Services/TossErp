namespace TossErp.Stock.Application.Common.DTOs;

public class ItemTaxDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string TaxType { get; set; } = string.Empty;
    public string TaxCategory { get; set; } = string.Empty;
    public string TaxCode { get; set; } = string.Empty;
    public decimal TaxRate { get; set; }
    public bool IsDefault { get; set; }
    public bool Disabled { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
} 
