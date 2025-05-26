namespace Catalog.Application.DTOs;

public class ProductTagDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ProductCount { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? UpdatedOnUtc { get; set; }
} 
