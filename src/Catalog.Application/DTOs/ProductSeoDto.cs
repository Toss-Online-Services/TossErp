namespace Catalog.Application.DTOs;

public class ProductSeoDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string MetaTitle { get; set; }
    public string MetaKeywords { get; set; }
    public string MetaDescription { get; set; }
    public string SeName { get; set; }
} 
