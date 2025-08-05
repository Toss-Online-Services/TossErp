namespace TossErp.Stock.Application.Common.DTOs;

public class StockEntryDetailDto
{
    public Guid Id { get; set; }
    public Guid StockEntryId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UOM { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal TransferQty { get; set; }
    public decimal ConsumedQty { get; set; }
    public decimal BasicRate { get; set; }
    public decimal BasicAmount { get; set; }
    public decimal AdditionalCost { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public string SourceWarehouse { get; set; } = string.Empty;
    public string TargetWarehouse { get; set; } = string.Empty;
    public string SourceBin { get; set; } = string.Empty;
    public string TargetBin { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public string SerialNo { get; set; } = string.Empty;
    public string CostCenter { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    public bool AllowZeroValuation { get; set; }
    public bool IsFinishedGood { get; set; }
    public bool IsProcessLoss { get; set; }
    public bool IsScrapItem { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
} 
