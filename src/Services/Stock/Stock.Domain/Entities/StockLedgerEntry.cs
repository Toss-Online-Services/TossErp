using TossErp.Stock.Domain.Events;
using TossErp.Stock.Domain.ValueObjects;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate.Entities;

namespace TossErp.Stock.Domain.Entities;

public class StockLedgerEntry : BaseEntity
{
    public ItemAggregate Item { get; private set; } = null!;
    public WarehouseAggregate Warehouse { get; private set; } = null!;
    public Bin Bin { get; private set; } = null!;
    public string ItemCode { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string BinName { get; private set; } = string.Empty;
    public DateTime PostingDate { get; private set; }
    public DateTime PostingTime { get; private set; }
    public string VoucherType { get; private set; } = string.Empty;
    public string VoucherNo { get; private set; } = string.Empty;
    public string? VoucherDetailNo { get; private set; }
    public Quantity Qty { get; private set; } = null!;
    public Rate ValuationRate { get; private set; } = null!;
    public Rate StockValue { get; private set; } = null!;
    public string? SerialNo { get; private set; }
    public string? BatchNo { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public string? Project { get; private set; }
    public string? CostCenter { get; private set; }
    public string? Company { get; private set; }
    public string? FiscalYear { get; private set; }
    public string? StockUOM { get; private set; }
    public decimal ConversionFactor { get; private set; }
    public string? ReferenceDocumentType { get; private set; }
    public string? ReferenceDocumentNo { get; private set; }
    public string? ReferenceDocumentDetailNo { get; private set; }
    public string? Remarks { get; private set; }
    public bool IsCancelled { get; private set; }
    public DateTime? CancelledDate { get; private set; }
    public string? CancelledBy { get; private set; }
    public string? CancellationReason { get; private set; }
    public bool IsDisabled { get; private set; }

    private StockLedgerEntry() { } // For EF Core

    public StockLedgerEntry(
        ItemAggregate item,
        WarehouseAggregate warehouse,
        Bin bin,
        DateTime postingDate,
        string voucherType,
        string voucherNo,
        Quantity qty,
        Rate valuationRate)
    {
        Item = item ?? throw new ArgumentNullException(nameof(item));
        Warehouse = warehouse ?? throw new ArgumentNullException(nameof(warehouse));
        Bin = bin ?? throw new ArgumentNullException(nameof(bin));
        ItemCode = item.ItemCode.Value;
        WarehouseCode = warehouse.Code.Value;
        BinName = bin.BinCode.Value;
        PostingDate = postingDate;
        PostingTime = DateTime.UtcNow;
        VoucherType = voucherType ?? throw new ArgumentNullException(nameof(voucherType));
        VoucherNo = voucherNo ?? throw new ArgumentNullException(nameof(voucherNo));
        Qty = qty;
        ValuationRate = valuationRate;
        StockValue = new Rate(qty.Value * valuationRate.Value);
        ConversionFactor = 1;
        IsCancelled = false;
        IsDisabled = false;

        AddDomainEvent(new StockLedgerEntryCreatedEvent(this));
    }

    public void UpdateVoucherDetail(string? voucherDetailNo)
    {
        VoucherDetailNo = voucherDetailNo;
        AddDomainEvent(new StockLedgerEntryVoucherDetailUpdatedEvent(this));
    }

    public void UpdateSerialBatchInfo(string? serialNo, string? batchNo, DateTime? expiryDate)
    {
        SerialNo = serialNo;
        BatchNo = batchNo;
        ExpiryDate = expiryDate;
        AddDomainEvent(new StockLedgerEntrySerialBatchUpdatedEvent(this));
    }

    public void UpdateAccountSettings(string? project, string? costCenter)
    {
        Project = project;
        CostCenter = costCenter;
        AddDomainEvent(new StockLedgerEntryAccountSettingsUpdatedEvent(this));
    }

    public void UpdateCompanySettings(string? company, string? fiscalYear)
    {
        Company = company;
        FiscalYear = fiscalYear;
        AddDomainEvent(new StockLedgerEntryCompanySettingsUpdatedEvent(this));
    }

    public void UpdateUOMSettings(string? stockUOM, decimal conversionFactor)
    {
        StockUOM = stockUOM;
        ConversionFactor = conversionFactor > 0 ? conversionFactor : throw new ArgumentException("Conversion factor must be positive", nameof(conversionFactor));
        AddDomainEvent(new StockLedgerEntryUOMSettingsUpdatedEvent(this));
    }

    public void UpdateReferenceDocument(string? referenceDocumentType, string? referenceDocumentNo, string? referenceDocumentDetailNo)
    {
        ReferenceDocumentType = referenceDocumentType;
        ReferenceDocumentNo = referenceDocumentNo;
        ReferenceDocumentDetailNo = referenceDocumentDetailNo;
        AddDomainEvent(new StockLedgerEntryReferenceDocumentUpdatedEvent(this));
    }

    public void UpdateRemarks(string? remarks)
    {
        Remarks = remarks;
        AddDomainEvent(new StockLedgerEntryRemarksUpdatedEvent(this));
    }

    public void Cancel(string cancelledBy, string? cancellationReason = null)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Stock ledger entry is already cancelled");

        IsCancelled = true;
        CancelledDate = DateTime.UtcNow;
        CancelledBy = cancelledBy ?? throw new ArgumentNullException(nameof(cancelledBy));
        CancellationReason = cancellationReason;
        AddDomainEvent(new StockLedgerEntryCancelledEvent(this));
    }

    public void Disable()
    {
        IsDisabled = true;
        AddDomainEvent(new StockLedgerEntryDisabledEvent(this));
    }

    public void Enable()
    {
        IsDisabled = false;
        AddDomainEvent(new StockLedgerEntryEnabledEvent(this));
    }

    public bool IsIncoming()
    {
        return Qty.Value > 0;
    }

    public bool IsOutgoing()
    {
        return Qty.Value < 0;
    }

    public bool IsCancelledEntry()
    {
        return IsCancelled;
    }

    public bool IsDisabledEntry()
    {
        return IsDisabled;
    }

    public bool HasSerialNumber()
    {
        return !string.IsNullOrEmpty(SerialNo);
    }

    public bool HasBatchNumber()
    {
        return !string.IsNullOrEmpty(BatchNo);
    }

    public bool HasExpiryDate()
    {
        return ExpiryDate.HasValue;
    }

    public bool IsExpired()
    {
        return ExpiryDate.HasValue && ExpiryDate.Value <= DateTime.UtcNow;
    }

    public bool HasReferenceDocument()
    {
        return !string.IsNullOrEmpty(ReferenceDocumentType) && !string.IsNullOrEmpty(ReferenceDocumentNo);
    }

    public bool HasProject()
    {
        return !string.IsNullOrEmpty(Project);
    }

    public bool HasCostCenter()
    {
        return !string.IsNullOrEmpty(CostCenter);
    }

    public decimal GetStockValue()
    {
        return StockValue.Value;
    }

    public string GetVoucherReference()
    {
        return $"{VoucherType}-{VoucherNo}";
    }

    public string GetReferenceDocumentReference()
    {
        if (string.IsNullOrEmpty(ReferenceDocumentType) || string.IsNullOrEmpty(ReferenceDocumentNo))
            return string.Empty;

        return $"{ReferenceDocumentType}-{ReferenceDocumentNo}";
    }
} 
