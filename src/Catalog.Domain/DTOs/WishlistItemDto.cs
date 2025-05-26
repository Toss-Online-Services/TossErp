namespace Catalog.Domain.DTOs;

public class WishlistItemDto
{
    public int Id { get; set; }
    public int CatalogItemId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime DateAdded { get; set; }
    public CatalogItemDto? CatalogItem { get; set; }
} 
