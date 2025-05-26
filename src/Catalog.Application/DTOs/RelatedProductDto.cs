namespace Catalog.Application.DTOs;

public class RelatedProductDto
{
    public int Id { get; set; }
    public int ProductId1 { get; set; }
    public int ProductId2 { get; set; }
    public int DisplayOrder { get; set; }
    public ProductDto Product2 { get; set; }
} 
