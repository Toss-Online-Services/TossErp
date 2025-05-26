namespace Catalog.Application.DTOs;

public class ProductBundleDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Published { get; set; }
    public int DisplayOrder { get; set; }
    public List<ProductBundleItemDto> BundleItems { get; set; }
} 
