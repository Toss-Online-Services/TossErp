namespace Catalog.Application.DTOs;

public class ProductAttributeMappingDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int ProductAttributeId { get; set; }
    public string TextPrompt { get; set; }
    public bool IsRequired { get; set; }
    public int AttributeControlTypeId { get; set; }
    public int DisplayOrder { get; set; }
    public string ProductAttributeName { get; set; }
    public List<ProductAttributeValueDto> ProductAttributeValues { get; set; }
} 
