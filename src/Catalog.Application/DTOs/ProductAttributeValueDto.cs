namespace Catalog.Application.DTOs;

public class ProductAttributeValueDto
{
    public int Id { get; set; }
    public int ProductAttributeMappingId { get; set; }
    public string Name { get; set; }
    public string ColorSquaresRgb { get; set; }
    public decimal PriceAdjustment { get; set; }
    public decimal WeightAdjustment { get; set; }
    public decimal Cost { get; set; }
    public int Quantity { get; set; }
    public bool IsPreSelected { get; set; }
    public int DisplayOrder { get; set; }
    public string PictureUrl { get; set; }
} 
