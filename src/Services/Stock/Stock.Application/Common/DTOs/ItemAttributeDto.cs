namespace TossErp.Stock.Application.Common.DTOs;

public class ItemAttributeDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string Attribute { get; set; } = string.Empty;
    public string AttributeValue { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
} 
