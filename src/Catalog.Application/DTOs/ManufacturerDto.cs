namespace Catalog.Application.DTOs;

public class ManufacturerDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int PictureId { get; set; }
    public string PictureUrl { get; set; }
    public bool ShowOnHomepage { get; set; }
    public bool Published { get; set; }
    public bool Deleted { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? UpdatedOnUtc { get; set; }
    public List<ProductManufacturerDto> ProductManufacturers { get; set; }
} 
