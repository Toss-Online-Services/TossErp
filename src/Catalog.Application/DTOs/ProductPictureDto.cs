namespace Catalog.Application.DTOs;

public class ProductPictureDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string PictureUrl { get; set; }
    public string AltAttribute { get; set; }
    public string TitleAttribute { get; set; }
    public int DisplayOrder { get; set; }
} 
