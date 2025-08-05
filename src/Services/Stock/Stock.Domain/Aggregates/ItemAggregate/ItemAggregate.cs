using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.ValueObjects;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;
using TossErp.Stock.Domain.Aggregates.ItemAggregate.Events;

// Type aliases to resolve ambiguous references
using AggregateItemCreatedEvent = TossErp.Stock.Domain.Aggregates.ItemAggregate.Events.ItemCreatedEvent;
using AggregateItemBasicInfoUpdatedEvent = TossErp.Stock.Domain.Aggregates.ItemAggregate.Events.ItemBasicInfoUpdatedEvent;
using AggregateItemPricingUpdatedEvent = TossErp.Stock.Domain.Aggregates.ItemAggregate.Events.ItemPricingUpdatedEvent;
using AggregateItemInventorySettingsUpdatedEvent = TossErp.Stock.Domain.Aggregates.ItemAggregate.Events.ItemInventorySettingsUpdatedEvent;
using AggregateItemValuationMethodUpdatedEvent = TossErp.Stock.Domain.Aggregates.ItemAggregate.Events.ItemValuationMethodUpdatedEvent;
using AggregateItemDisabledEvent = TossErp.Stock.Domain.Aggregates.ItemAggregate.Events.ItemDisabledEvent;
using AggregateItemEnabledEvent = TossErp.Stock.Domain.Aggregates.ItemAggregate.Events.ItemEnabledEvent;
using AggregateItemDeletedEvent = TossErp.Stock.Domain.Aggregates.ItemAggregate.Events.ItemDeletedEvent;
using AggregateItemRestoredEvent = TossErp.Stock.Domain.Aggregates.ItemAggregate.Events.ItemRestoredEvent;
using AggregateItemVariantAddedEvent = TossErp.Stock.Domain.Aggregates.ItemAggregate.Events.ItemVariantAddedEvent;
using AggregateItemVariantRemovedEvent = TossErp.Stock.Domain.Aggregates.ItemAggregate.Events.ItemVariantRemovedEvent;
using AggregateItemPriceAddedEvent = TossErp.Stock.Domain.Aggregates.ItemAggregate.Events.ItemPriceAddedEvent;
using AggregateItemPriceRemovedEvent = TossErp.Stock.Domain.Aggregates.ItemAggregate.Events.ItemPriceRemovedEvent;
using AggregateItemSupplierAddedEvent = TossErp.Stock.Domain.Aggregates.ItemAggregate.Events.ItemSupplierAddedEvent;
using AggregateItemSupplierRemovedEvent = TossErp.Stock.Domain.Aggregates.ItemAggregate.Events.ItemSupplierRemovedEvent;

namespace TossErp.Stock.Domain.Aggregates.ItemAggregate;

/// <summary>
/// Item Aggregate Root
/// Manages all item-related business logic and child entities
/// </summary>
public class ItemAggregate : Entity, IAggregateRoot
{
    // Core Properties
    public ItemCode ItemCode { get; private set; } = null!;
    public string ItemName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string ItemGroup { get; private set; } = string.Empty;
    public string? Brand { get; private set; }
    public UnitOfMeasure StockUOM { get; private set; } = null!;
    public ItemType ItemType { get; private set; }
    public ValuationMethod ValuationMethod { get; private set; }
    public ItemStatus ItemStatus { get; private set; }
    public PriorityLevel PriorityLevel { get; private set; }
    public string Company { get; private set; } = string.Empty;

    // Pricing
    public decimal? StandardRate { get; private set; }
    public decimal? LastPurchaseRate { get; private set; }
    public decimal? BaseRate { get; private set; }
    public decimal? MinimumPrice { get; private set; }

    // Inventory Settings
    public decimal? WeightPerUnit { get; private set; }
    public decimal? WeightUOM { get; private set; }
    public decimal? ReOrderLevel { get; private set; }
    public decimal? ReOrderQty { get; private set; }
    public decimal? MaxQty { get; private set; }
    public decimal? MinQty { get; private set; }

    // Physical Dimensions
    public decimal? Length { get; private set; }
    public decimal? Width { get; private set; }
    public decimal? Height { get; private set; }

    // Flags
    public bool IsStockItem { get; private set; }
    public bool Disabled { get; private set; }
    public bool Deleted { get; private set; }

    // Child Collections
    private readonly List<ItemVariant> _variants = new();
    private readonly List<ItemPrice> _prices = new();
    private readonly List<ItemSupplier> _suppliers = new();
    private readonly List<ItemBarcode> _barcodes = new();
    private readonly List<ItemTax> _taxes = new();
    private readonly List<ItemReorder> _reorderLevels = new();
    private readonly List<ItemAttribute> _attributes = new();
    private readonly List<ItemAlternative> _alternatives = new();
    private readonly List<ItemManufacturer> _manufacturers = new();
    private readonly List<UOMConversionDetail> _uomConversions = new();

    // Navigation Properties
    public IReadOnlyCollection<ItemVariant> Variants => _variants.AsReadOnly();
    public IReadOnlyCollection<ItemPrice> Prices => _prices.AsReadOnly();
    public IReadOnlyCollection<ItemSupplier> Suppliers => _suppliers.AsReadOnly();
    public IReadOnlyCollection<ItemBarcode> Barcodes => _barcodes.AsReadOnly();
    public IReadOnlyCollection<ItemTax> Taxes => _taxes.AsReadOnly();
    public IReadOnlyCollection<ItemReorder> ReorderLevels => _reorderLevels.AsReadOnly();
    public IReadOnlyCollection<ItemAttribute> Attributes => _attributes.AsReadOnly();
    public IReadOnlyCollection<ItemAlternative> Alternatives => _alternatives.AsReadOnly();
    public IReadOnlyCollection<ItemManufacturer> Manufacturers => _manufacturers.AsReadOnly();
    public IReadOnlyCollection<UOMConversionDetail> UOMConversions => _uomConversions.AsReadOnly();

    // Computed Properties (calculated from StockLedgerEntry)
    public decimal CurrentStock { get; set; } = 0; // This will be set by the repository or service layer
    public Guid ItemId => Id; // Alias for compatibility

    protected ItemAggregate() { } // For EF Core

    public ItemAggregate(
        ItemCode itemCode,
        string itemName,
        string itemGroup,
        UnitOfMeasure stockUOM,
        ItemType itemType,
        ValuationMethod valuationMethod,
        string company)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            throw new ArgumentException("Item name cannot be empty", nameof(itemName));
        if (string.IsNullOrWhiteSpace(itemGroup))
            throw new ArgumentException("Item group cannot be empty", nameof(itemGroup));
        if (string.IsNullOrWhiteSpace(company))
            throw new ArgumentException("Company cannot be empty", nameof(company));

        ItemCode = itemCode;
        ItemName = itemName;
        ItemGroup = itemGroup;
        StockUOM = stockUOM;
        ItemType = itemType;
        ValuationMethod = valuationMethod;
        Company = company;
        ItemStatus = ItemStatus.Active;
        PriorityLevel = PriorityLevel.Normal;
        IsStockItem = itemType == ItemType.Stock;
        Disabled = false;
        Deleted = false;

        AddDomainEvent(new AggregateItemCreatedEvent(this));
    }

    // Business Methods
    public void UpdateBasicInfo(string itemName, string? description, string? brand, string? itemGroup = null)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            throw new ArgumentException("Item name cannot be empty", nameof(itemName));

        ItemName = itemName;
        Description = description;
        Brand = brand;
        if (!string.IsNullOrWhiteSpace(itemGroup))
            ItemGroup = itemGroup;

        AddDomainEvent(new AggregateItemBasicInfoUpdatedEvent(this));
    }

    public void UpdatePricing(decimal? standardRate, decimal? minimumPrice)
    {
        if (standardRate.HasValue && standardRate.Value < 0)
            throw new ArgumentException("Standard rate cannot be negative", nameof(standardRate));
        if (minimumPrice.HasValue && minimumPrice.Value < 0)
            throw new ArgumentException("Minimum price cannot be negative", nameof(minimumPrice));

        StandardRate = standardRate;
        MinimumPrice = minimumPrice;
        AddDomainEvent(new ItemPricingUpdatedEvent(this));
    }

    public void UpdateInventorySettings(
        decimal? reOrderLevel, 
        decimal? reOrderQty, 
        decimal? maxQty, 
        decimal? minQty)
    {
        if (reOrderLevel.HasValue && reOrderLevel.Value < 0)
            throw new ArgumentException("Reorder level cannot be negative", nameof(reOrderLevel));
        if (reOrderQty.HasValue && reOrderQty.Value <= 0)
            throw new ArgumentException("Reorder quantity must be positive", nameof(reOrderQty));

        ReOrderLevel = reOrderLevel;
        ReOrderQty = reOrderQty;
        MaxQty = maxQty;
        MinQty = minQty;
        AddDomainEvent(new ItemInventorySettingsUpdatedEvent(this));
    }

    public void UpdateValuationMethod(ValuationMethod valuationMethod)
    {
        ValuationMethod = valuationMethod;
        AddDomainEvent(new ItemValuationMethodUpdatedEvent(this));
    }

    public void Disable()
    {
        if (Disabled)
            throw new InvalidOperationException("Item is already disabled");

        Disabled = true;
        AddDomainEvent(new ItemDisabledEvent(this));
    }

    public void Enable()
    {
        if (!Disabled)
            throw new InvalidOperationException("Item is not disabled");

        Disabled = false;
        AddDomainEvent(new ItemEnabledEvent(this));
    }

    public void SoftDelete()
    {
        if (Deleted)
            throw new InvalidOperationException("Item is already deleted");

        Deleted = true;
        AddDomainEvent(new ItemDeletedEvent(this));
    }

    public void Restore()
    {
        if (!Deleted)
            throw new InvalidOperationException("Item is not deleted");

        Deleted = false;
        AddDomainEvent(new ItemRestoredEvent(this));
    }

    // Child Entity Management
    public void AddVariant(ItemVariant variant)
    {
        if (variant == null)
            throw new ArgumentNullException(nameof(variant));

        if (_variants.Any(v => v.VariantCode == variant.VariantCode))
            throw new InvalidOperationException($"Variant with code {variant.VariantCode} already exists");

        _variants.Add(variant);
        AddDomainEvent(new ItemVariantAddedEvent(this, variant));
    }

    public void RemoveVariant(Guid variantId)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant == null)
            throw new InvalidOperationException($"Variant with id {variantId} not found");

        _variants.Remove(variant);
        AddDomainEvent(new ItemVariantRemovedEvent(this, variant));
    }

    public void AddPrice(ItemPrice price)
    {
        if (price == null)
            throw new ArgumentNullException(nameof(price));

        if (_prices.Any(p => p.PriceList == price.PriceList && p.Currency == price.Currency))
            throw new InvalidOperationException($"Price for {price.PriceList} in {price.Currency} already exists");

        _prices.Add(price);
        AddDomainEvent(new ItemPriceAddedEvent(this, price));
    }

    public void RemovePrice(Guid priceId)
    {
        var price = _prices.FirstOrDefault(p => p.Id == priceId);
        if (price == null)
            throw new InvalidOperationException($"Price with id {priceId} not found");

        _prices.Remove(price);
        AddDomainEvent(new ItemPriceRemovedEvent(this, price));
    }

    public void AddSupplier(ItemSupplier supplier)
    {
        if (supplier == null)
            throw new ArgumentNullException(nameof(supplier));

        if (_suppliers.Any(s => s.SupplierId == supplier.SupplierId))
            throw new InvalidOperationException($"Supplier {supplier.SupplierId} already exists for this item");

        _suppliers.Add(supplier);
        AddDomainEvent(new ItemSupplierAddedEvent(this, supplier));
    }

    public void RemoveSupplier(Guid supplierId)
    {
        var supplier = _suppliers.FirstOrDefault(s => s.SupplierId == supplierId);
        if (supplier == null)
            throw new InvalidOperationException($"Supplier with id {supplierId} not found");

        _suppliers.Remove(supplier);
        AddDomainEvent(new ItemSupplierRemovedEvent(this, supplier));
    }

    // Business Rules
    public bool IsAvailableForSale() => 
        !Disabled && !Deleted && ItemStatus == ItemStatus.Active;

    public bool IsAvailableForPurchase() => 
        !Disabled && !Deleted && ItemStatus == ItemStatus.Active;

    public bool RequiresReorder() => 
        ReOrderLevel.HasValue && ReOrderLevel.Value > 0;

    public decimal GetCurrentPrice(string priceList, string currency)
    {
        var price = _prices.FirstOrDefault(p => 
            p.PriceList == priceList && p.Currency == currency);
        return price?.Rate ?? StandardRate ?? 0;
    }

    public bool HasVariant(string variantCode) => 
        _variants.Any(v => v.VariantCode == variantCode);

    public bool HasSupplier(Guid supplierId) => 
        _suppliers.Any(s => s.SupplierId == supplierId);
} 
