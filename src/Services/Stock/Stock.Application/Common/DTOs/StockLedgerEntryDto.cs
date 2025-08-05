namespace TossErp.Stock.Application.Common.DTOs;

public class StockLedgerEntryDto
{
    public Guid Id { get; set; }
    public string VoucherType { get; set; } = string.Empty;
    public string VoucherNo { get; set; } = string.Empty;
    public DateTime PostingDate { get; set; }
    public DateTime PostingTime { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string Warehouse { get; set; } = string.Empty;
    public string Bin { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public string SerialNo { get; set; } = string.Empty;
    public string UOM { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal BalanceQty { get; set; }
    public decimal IncomingRate { get; set; }
    public decimal ValuationRate { get; set; }
    public decimal BalanceValue { get; set; }
    public decimal StockValue { get; set; }
    public string Company { get; set; } = string.Empty;
    public string FiscalYear { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public string CostCenter { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    public bool IsCancelled { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
} 
