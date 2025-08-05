using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Domain.Aggregates.StockEntryAggregate.Entities;

/// <summary>
/// Stock Entry Detail - Child entity of StockEntry Aggregate
/// Represents individual stock movement line items
/// </summary>
public class StockEntryDetail : Entity
{
    public Guid StockEntryId { get; private set; }
    public Guid ItemId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid? BinId { get; private set; }
    public Quantity Quantity { get; private set; } = null!;
    public Rate Rate { get; private set; } = null!;
    public string? BatchNo { get; private set; }
    public string? SerialNo { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public string? Remarks { get; private set; }
    public bool IsValid { get; private set; }

    protected StockEntryDetail() { } // For EF Core

    public StockEntryDetail(
        Guid stockEntryId,
        Guid itemId,
        Guid warehouseId,
        Quantity quantity,
        Rate rate,
        Guid? binId = null,
        string? batchNo = null,
        string? serialNo = null,
        DateTime? expiryDate = null,
        string? remarks = null)
    {
        if (stockEntryId == Guid.Empty)
            throw new ArgumentException("Stock entry ID cannot be empty", nameof(stockEntryId));
        if (itemId == Guid.Empty)
            throw new ArgumentException("Item ID cannot be empty", nameof(itemId));
        if (warehouseId == Guid.Empty)
            throw new ArgumentException("Warehouse ID cannot be empty", nameof(warehouseId));

        StockEntryId = stockEntryId;
        ItemId = itemId;
        WarehouseId = warehouseId;
        Quantity = quantity;
        Rate = rate;
        BinId = binId;
        BatchNo = batchNo?.Trim();
        SerialNo = serialNo?.Trim();
        ExpiryDate = expiryDate;
        Remarks = remarks?.Trim();
        IsValid = true;
    }

    public void UpdateQuantity(Quantity newQuantity)
    {
        if (newQuantity == null)
            throw new ArgumentNullException(nameof(newQuantity));

        Quantity = newQuantity;
    }

    public void UpdateRate(Rate newRate)
    {
        if (newRate == null)
            throw new ArgumentNullException(nameof(newRate));

        Rate = newRate;
    }

    public void UpdateLocation(Guid warehouseId, Guid? binId = null)
    {
        if (warehouseId == Guid.Empty)
            throw new ArgumentException("Warehouse ID cannot be empty", nameof(warehouseId));

        WarehouseId = warehouseId;
        BinId = binId;
    }

    public void UpdateBatchInfo(string? batchNo, DateTime? expiryDate)
    {
        BatchNo = batchNo?.Trim();
        ExpiryDate = expiryDate;
    }

    public void UpdateSerialNo(string? serialNo)
    {
        SerialNo = serialNo?.Trim();
    }

    public void UpdateRemarks(string? remarks)
    {
        Remarks = remarks?.Trim();
    }

    public void MarkAsValid()
    {
        IsValid = true;
    }

    public void MarkAsInvalid()
    {
        IsValid = false;
    }

    public decimal GetTotalValue()
    {
        return Quantity.Value * Rate.Value;
    }

    public bool HasBatch() => !string.IsNullOrWhiteSpace(BatchNo);

    public bool HasSerialNo() => !string.IsNullOrWhiteSpace(SerialNo);

    public bool IsExpired(DateTime? asOfDate = null)
    {
        if (!ExpiryDate.HasValue) return false;
        var checkDate = asOfDate ?? DateTime.UtcNow;
        return checkDate > ExpiryDate.Value;
    }

    public bool IsExpiringSoon(int daysThreshold = 30, DateTime? asOfDate = null)
    {
        if (!ExpiryDate.HasValue) return false;
        var checkDate = asOfDate ?? DateTime.UtcNow;
        return ExpiryDate.Value <= checkDate.AddDays(daysThreshold) && 
               ExpiryDate.Value > checkDate;
    }
} 
