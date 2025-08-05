namespace TossErp.Stock.Application.Common.DTOs;

public class StockEntryAdditionalCostDto
{
    public Guid Id { get; set; }
    public Guid StockEntryId { get; set; }
    public string ExpenseAccount { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CostCenter { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
} 
