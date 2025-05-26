namespace Catalog.Application.DTOs;

public class ProductBundleItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int BundleProductId { get; set; }
    public int Quantity { get; set; }
    public string Name { get; set; }
    public string ShortDescription { get; set; }
    public string PictureUrl { get; set; }
    public decimal Price { get; set; }
    public int DisplayOrder { get; set; }
} 
