namespace Catalog.Application.DTOs;

public class ProductSpecificationAttributeDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int SpecificationAttributeOptionId { get; set; }
    public string SpecificationAttributeName { get; set; }
    public string SpecificationAttributeOptionName { get; set; }
    public bool AllowFiltering { get; set; }
    public bool ShowOnProductPage { get; set; }
    public int DisplayOrder { get; set; }
} 
