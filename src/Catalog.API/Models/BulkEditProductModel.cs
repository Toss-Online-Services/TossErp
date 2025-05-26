namespace Catalog.API.Models;

public class BulkEditProductModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Money? Price { get; set; }
    public string? PictureUri { get; set; }
    public int? CatalogTypeId { get; set; }
    public int? CatalogBrandId { get; set; }
    public int? AvailableStock { get; set; }
    public bool? IsPublished { get; set; }
} 
