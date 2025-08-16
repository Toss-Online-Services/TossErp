using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.ValueObjects;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate.Entities;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate.Events;
using TossErp.Stock.Domain.Enums;

namespace TossErp.Stock.Domain.Aggregates.StockEntryAggregate;

/// <summary>
/// Stock Entry Aggregate Root
/// Manages stock movements and transactions
/// </summary>
public class StockEntryAggregate : Entity, IAggregateRoot
{
    public StockEntryNo EntryNumber { get; private set; } = null!;
    public DateTime EntryDate { get; private set; }
    public string? Reference { get; private set; }
    public string? Notes { get; private set; }
    public bool IsPosted { get; private set; }
    public DateTime? PostedDate { get; private set; }
    public string? PostedBy { get; private set; }
    public string Company { get; private set; } = string.Empty;
    public Guid? StockEntryTypeId { get; private set; }
    public StockEntryStatus Status { get; private set; }

    // Child Collections
    private readonly List<StockEntryDetail> _details = new();
    private readonly List<StockEntryAdditionalCost> _additionalCosts = new();

    // Navigation Properties
    public IReadOnlyCollection<StockEntryDetail> Details => _details.AsReadOnly();
    public IReadOnlyCollection<StockEntryAdditionalCost> AdditionalCosts => _additionalCosts.AsReadOnly();

    protected StockEntryAggregate() { } // For EF Core

    public StockEntryAggregate(
        StockEntryNo entryNumber, 
        DateTime entryDate, 
        string company,
        string? reference = null, 
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(company))
            throw new ArgumentException("Company cannot be empty", nameof(company));

        EntryNumber = entryNumber;
        EntryDate = entryDate;
        Company = company;
        Reference = reference?.Trim();
        Notes = notes?.Trim();
        IsPosted = false;
        Status = StockEntryStatus.Draft;

        AddDomainEvent(new StockEntryCreatedEvent(this));
    }

    // Business Methods
    public void AddDetail(StockEntryDetail detail)
    {
        if (detail == null)
            throw new ArgumentNullException(nameof(detail));

        if (IsPosted)
            throw new InvalidOperationException("Cannot add details to a posted entry.");

        // Validate that the detail belongs to this entry
        if (detail.StockEntryId != Id)
            throw new InvalidOperationException("Detail does not belong to this stock entry.");

        _details.Add(detail);
        AddDomainEvent(new StockEntryDetailAddedEvent(this, detail));
    }

    public void RemoveDetail(Guid detailId)
    {
        if (IsPosted)
            throw new InvalidOperationException("Cannot remove details from a posted entry.");

        var detail = _details.FirstOrDefault(d => d.Id == detailId);
        if (detail == null)
            throw new InvalidOperationException($"Detail with id {detailId} not found.");

        _details.Remove(detail);
        AddDomainEvent(new StockEntryDetailRemovedEvent(this, detail));
    }

    public void AddAdditionalCost(StockEntryAdditionalCost additionalCost)
    {
        if (additionalCost == null)
            throw new ArgumentNullException(nameof(additionalCost));

        if (IsPosted)
            throw new InvalidOperationException("Cannot add additional costs to a posted entry.");

        _additionalCosts.Add(additionalCost);
        AddDomainEvent(new StockEntryAdditionalCostAddedEvent(this, additionalCost));
    }

    public void RemoveAdditionalCost(Guid additionalCostId)
    {
        if (IsPosted)
            throw new InvalidOperationException("Cannot remove additional costs from a posted entry.");

        var additionalCost = _additionalCosts.FirstOrDefault(ac => ac.Id == additionalCostId);
        if (additionalCost == null)
            throw new InvalidOperationException($"Additional cost with id {additionalCostId} not found.");

        _additionalCosts.Remove(additionalCost);
        AddDomainEvent(new StockEntryAdditionalCostRemovedEvent(this, additionalCost));
    }

    public void Post(string postedBy)
    {
        if (IsPosted)
            throw new InvalidOperationException("Entry is already posted.");

        if (!_details.Any())
            throw new InvalidOperationException("Cannot post entry without details.");

        // Validate all details are valid
        foreach (var detail in _details)
        {
            if (!detail.IsValid)
                throw new InvalidOperationException($"Detail {detail.Id} is not valid for posting.");
        }

        IsPosted = true;
        PostedDate = DateTime.UtcNow;
        PostedBy = postedBy;

        AddDomainEvent(new StockEntryPostedEvent(this));
    }

    public void Update(string? reference = null, string? notes = null)
    {
        if (IsPosted)
            throw new InvalidOperationException("Cannot update a posted entry.");

        Reference = reference?.Trim();
        Notes = notes?.Trim();

        AddDomainEvent(new StockEntryUpdatedEvent(this));
    }

    public void SetStockEntryType(Guid? stockEntryTypeId)
    {
        if (IsPosted)
            throw new InvalidOperationException("Cannot change stock entry type of a posted entry.");

        StockEntryTypeId = stockEntryTypeId;
        AddDomainEvent(new StockEntryUpdatedEvent(this));
    }

    public void Delete()
    {
        if (IsPosted)
            throw new InvalidOperationException("Cannot delete a posted entry.");

        AddDomainEvent(new StockEntryDeletedEvent(this));
    }

    // Business Rules
    public bool CanBePosted() => 
        !IsPosted && _details.Any() && _details.All(d => d.IsValid);

    public bool HasDetails() => _details.Any();

    public decimal GetTotalQuantity()
    {
        return _details.Sum(d => d.Quantity.Value);
    }

    public decimal GetTotalValue()
    {
        return _details.Sum(d => d.Quantity.Value * d.Rate) + 
               _additionalCosts.Sum(ac => ac.Amount);
    }

    public bool HasAdditionalCosts() => _additionalCosts.Any();

    public IEnumerable<StockEntryDetail> GetDetailsByItem(Guid itemId)
    {
        return _details.Where(d => d.ItemId == itemId);
    }

    public IEnumerable<StockEntryDetail> GetDetailsByWarehouse(Guid warehouseId)
    {
        return _details.Where(d => d.WarehouseId == warehouseId);
    }

    /// <summary>
    /// Mark the stock entry as processed
    /// </summary>
    public void MarkAsProcessed()
    {
        if (IsPosted)
            throw new InvalidOperationException("Entry is already posted.");

        // Mark as processed (this could be a separate status from posted)
        // For now, we'll use the existing IsPosted flag
        IsPosted = true;
        PostedDate = DateTime.UtcNow;
        PostedBy = "System";
        Status = StockEntryStatus.Posted;

        AddDomainEvent(new StockEntryProcessedEvent(this));
    }

    /// <summary>
    /// Mark the stock entry as failed with an error message
    /// </summary>
    public void MarkAsFailed(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("Error message cannot be empty", nameof(errorMessage));

        // Add error information to notes
        Notes = $"Processing failed: {errorMessage}";
        Status = StockEntryStatus.Rejected;
        
        AddDomainEvent(new StockEntryFailedEvent(this, errorMessage));
    }
} 
