namespace Catalog.Application.DTOs;

public class ProductCategoryDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int CategoryId { get; set; }
    public bool IsFeaturedProduct { get; set; }
    public int DisplayOrder { get; set; }
    public string CategoryName { get; set; }
} 
