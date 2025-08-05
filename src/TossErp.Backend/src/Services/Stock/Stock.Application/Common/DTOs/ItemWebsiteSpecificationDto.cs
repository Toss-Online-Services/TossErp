namespace TossErp.Stock.Application.Common.DTOs;

public class ItemWebsiteSpecificationDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
} 
