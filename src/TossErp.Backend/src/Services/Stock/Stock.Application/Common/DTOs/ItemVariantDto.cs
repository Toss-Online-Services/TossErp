namespace TossErp.Stock.Application.Common.DTOs;

public class ItemVariantDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string VariantName { get; set; } = string.Empty;
    public string VariantCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Disabled { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    public List<ItemVariantAttributeDto> Attributes { get; set; } = new();
} 
