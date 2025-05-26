namespace Catalog.Application.DTOs;

public class ProductManufacturerDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int ManufacturerId { get; set; }
    public bool IsFeaturedProduct { get; set; }
    public int DisplayOrder { get; set; }
    public string ManufacturerName { get; set; }
} 
