namespace TossErp.Stock.Application.Common.DTOs;

public class ItemQualityInspectionParameterDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string Parameter { get; set; } = string.Empty;
    public string Specification { get; set; } = string.Empty;
    public string AcceptanceCriteria { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
} 
