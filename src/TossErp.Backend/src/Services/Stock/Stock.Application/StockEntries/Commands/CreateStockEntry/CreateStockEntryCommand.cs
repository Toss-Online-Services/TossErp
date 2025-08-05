using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.StockEntries.Commands.CreateStockEntry;

public record CreateStockEntryCommand : IRequest<StockEntryDto>
{
    public string StockEntryType { get; init; } = string.Empty;
    public string Purpose { get; init; } = string.Empty;
    public DateTime PostingDate { get; init; }
    public DateTime PostingTime { get; init; }
    public string Company { get; init; } = string.Empty;
    public string SourceWarehouse { get; init; } = string.Empty;
    public string TargetWarehouse { get; init; } = string.Empty;
    public string SourceCostCenter { get; init; } = string.Empty;
    public string TargetCostCenter { get; init; } = string.Empty;
    public string Project { get; init; } = string.Empty;
    public string ReferenceNo { get; init; } = string.Empty;
    public string ReferenceType { get; init; } = string.Empty;
    public string Remarks { get; init; } = string.Empty;
    public bool IsOpening { get; init; }
    public bool IsRepostItemValuation { get; init; }
    public bool IsRepostItemValuationAllowed { get; init; }
    public bool IsRepostItemValuationDone { get; init; }
    public List<CreateStockEntryDetailCommand> Details { get; init; } = new();
    public List<CreateStockEntryAdditionalCostCommand> AdditionalCosts { get; init; } = new();
}

public record CreateStockEntryDetailCommand
{
    public string ItemCode { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string UOM { get; init; } = string.Empty;
    public decimal Qty { get; init; }
    public decimal TransferQty { get; init; }
    public decimal ConsumedQty { get; init; }
    public decimal BasicRate { get; init; }
    public decimal BasicAmount { get; init; }
    public decimal AdditionalCost { get; init; }
    public decimal Rate { get; init; }
    public decimal Amount { get; init; }
    public string SourceWarehouse { get; init; } = string.Empty;
    public string TargetWarehouse { get; init; } = string.Empty;
    public string SourceBin { get; init; } = string.Empty;
    public string TargetBin { get; init; } = string.Empty;
    public string BatchNo { get; init; } = string.Empty;
    public string SerialNo { get; init; } = string.Empty;
    public string CostCenter { get; init; } = string.Empty;
    public string Project { get; init; } = string.Empty;
    public string Remarks { get; init; } = string.Empty;
    public bool AllowZeroValuation { get; init; }
    public bool IsFinishedGood { get; init; }
    public bool IsProcessLoss { get; init; }
    public bool IsScrapItem { get; init; }
}

public record CreateStockEntryAdditionalCostCommand
{
    public string ExpenseAccount { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string CostCenter { get; init; } = string.Empty;
    public string Project { get; init; } = string.Empty;
} 
