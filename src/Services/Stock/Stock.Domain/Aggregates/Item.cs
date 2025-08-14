using System;
using TossErp.Stock.Domain.SeedWork;
using TossErp.Stock.Domain.ValueObjects;
using TossErp.Stock.Domain.Events;
using System.Collections.Generic;
using System.Linq;

namespace TossErp.Stock.Domain.Aggregates;

public class Item : AggregateRoot<Guid>
{
    public Guid TenantId { get; private set; }
    public SKU SKU { get; private set; } = null!;
    public string? Barcode { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string Category { get; private set; } = null!;
    public string Unit { get; private set; } = null!;
    public Money SellingPrice { get; private set; } = null!;
    public Money? CostPrice { get; private set; }
    public Quantity ReorderLevel { get; private set; } = null!;
    public Quantity ReorderQty { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // For EF Core
    private Item() { }

    public static Item Create(
        Guid tenantId,
        string sku,
        string name,
        string category,
        string unit,
        decimal sellingPrice,
        decimal? costPrice = null,
        string? barcode = null,
        string? description = null,
        decimal reorderLevel = 0,
        decimal reorderQty = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Item name cannot be empty");

        if (string.IsNullOrWhiteSpace(category))
            throw new DomainException("Item category cannot be empty");

        if (string.IsNullOrWhiteSpace(unit))
            throw new DomainException("Item unit cannot be empty");

        if (sellingPrice <= 0)
            throw new DomainException("Selling price must be greater than zero");

        if (costPrice.HasValue && costPrice.Value < 0)
            throw new DomainException("Cost price cannot be negative");

        if (reorderLevel < 0)
            throw new DomainException("Reorder level cannot be negative");

        if (reorderQty < 0)
            throw new DomainException("Reorder quantity cannot be negative");

        var item = new Item
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            SKU = new SKU(sku),
            Barcode = barcode,
            Name = name,
            Description = description,
            Category = category,
            Unit = unit,
            SellingPrice = new Money(sellingPrice),
            CostPrice = costPrice.HasValue ? new Money(costPrice.Value) : null,
            ReorderLevel = new Quantity(reorderLevel, unit),
            ReorderQty = new Quantity(reorderQty, unit),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        item.AddDomainEvent(new ItemCreatedEvent(item.Id, item.TenantId, item.SKU.Value, item.Name, item.Category));

        return item;
    }

    public void Update(
        string? name = null,
        string? description = null,
        string? category = null,
        decimal? sellingPrice = null,
        decimal? costPrice = null,
        string? barcode = null)
    {
        if (name != null && string.IsNullOrWhiteSpace(name))
            throw new DomainException("Item name cannot be empty");

        if (category != null && string.IsNullOrWhiteSpace(category))
            throw new DomainException("Item category cannot be empty");

        if (sellingPrice.HasValue && sellingPrice.Value <= 0)
            throw new DomainException("Selling price must be greater than zero");

        if (costPrice.HasValue && costPrice.Value < 0)
            throw new DomainException("Cost price cannot be negative");

        var changes = new List<string>();

        if (name != null && name != Name)
        {
            Name = name;
            changes.Add("Name");
        }

        if (description != null && description != Description)
        {
            Description = description;
            changes.Add("Description");
        }

        if (category != null && category != Category)
        {
            Category = category;
            changes.Add("Category");
        }

        if (sellingPrice.HasValue && sellingPrice.Value != SellingPrice.Amount)
        {
            SellingPrice = new Money(sellingPrice.Value, SellingPrice.Currency);
            changes.Add("SellingPrice");
        }

        if (costPrice.HasValue && (CostPrice == null || costPrice.Value != CostPrice.Amount))
        {
            CostPrice = new Money(costPrice.Value, SellingPrice.Currency);
            changes.Add("CostPrice");
        }

        if (barcode != null && barcode != Barcode)
        {
            Barcode = barcode;
            changes.Add("Barcode");
        }

        if (changes.Any())
        {
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new ItemUpdatedEvent(Id, changes.ToArray()));
        }
    }

    public void UpdatePricing(decimal sellingPrice, decimal? costPrice = null)
    {
        if (sellingPrice <= 0)
            throw new DomainException("Selling price must be greater than zero");

        if (costPrice.HasValue && costPrice.Value < 0)
            throw new DomainException("Cost price cannot be negative");

        var changes = new List<string>();

        if (sellingPrice != SellingPrice.Amount)
        {
            SellingPrice = new Money(sellingPrice, SellingPrice.Currency);
            changes.Add("SellingPrice");
        }

        if (costPrice.HasValue && (CostPrice == null || costPrice.Value != CostPrice.Amount))
        {
            CostPrice = new Money(costPrice.Value, SellingPrice.Currency);
            changes.Add("CostPrice");
        }

        if (changes.Any())
        {
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new ItemUpdatedEvent(Id, changes.ToArray()));
        }
    }

    public void SetReorderLevels(decimal reorderLevel, decimal reorderQty)
    {
        if (reorderLevel < 0)
            throw new DomainException("Reorder level cannot be negative");

        if (reorderQty < 0)
            throw new DomainException("Reorder quantity cannot be negative");

        var changes = new List<string>();

        if (reorderLevel != ReorderLevel.Value)
        {
            ReorderLevel = new Quantity(reorderLevel, Unit);
            changes.Add("ReorderLevel");
        }

        if (reorderQty != ReorderQty.Value)
        {
            ReorderQty = new Quantity(reorderQty, Unit);
            changes.Add("ReorderQty");
        }

        if (changes.Any())
        {
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new ItemUpdatedEvent(Id, changes.ToArray()));
        }
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ItemDeactivatedEvent(Id));
    }

    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ItemUpdatedEvent(Id, new[] { "IsActive" }));
    }

    public bool IsLowStock(Quantity currentStock)
    {
        return currentStock < ReorderLevel;
    }

    public Quantity GetReorderQuantity()
    {
        return ReorderQty;
    }
}
