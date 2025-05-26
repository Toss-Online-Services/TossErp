namespace Catalog.Domain.DTOs;

public class CatalogFilterDto
{
    public int? BrandId { get; set; }
    public int? TypeId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SearchTerm { get; set; }
    public bool? InStock { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
} 
