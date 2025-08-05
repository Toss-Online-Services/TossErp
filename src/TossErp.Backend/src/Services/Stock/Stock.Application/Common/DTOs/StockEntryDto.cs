namespace TossErp.Stock.Application.Common.DTOs;

public class StockEntryDto
{
    public Guid Id { get; set; }
    public string StockEntryNo { get; set; } = string.Empty;
    public string StockEntryType { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public DateTime PostingDate { get; set; }
    public DateTime PostingTime { get; set; }
    public string Company { get; set; } = string.Empty;
    public string SourceWarehouse { get; set; } = string.Empty;
    public string TargetWarehouse { get; set; } = string.Empty;
    public string SourceCostCenter { get; set; } = string.Empty;
    public string TargetCostCenter { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public string ReferenceNo { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal TotalQty { get; set; }
    public bool IsOpening { get; set; }
    public bool IsRepostItemValuation { get; set; }
    public bool IsRepostItemValuationAllowed { get; set; }
    public bool IsRepostItemValuationDone { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    public List<StockEntryDetailDto> Details { get; set; } = new();
    public List<StockEntryAdditionalCostDto> AdditionalCosts { get; set; } = new();
} 
