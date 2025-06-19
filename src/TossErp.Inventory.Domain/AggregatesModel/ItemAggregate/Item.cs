using System;
using System.Collections.Generic;
using System.Linq;
using TossErp.Domain.SeedWork;
using TossErp.Inventory.Domain.Events;
using TossErp.Inventory.Domain.Enums;
using TossErp.Domain.Exceptions;

namespace TossErp.Inventory.Domain.AggregatesModel.ItemAggregate
{
    public class Item : Entity, IAggregateRoot
    {
        public string ItemCode { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string? Barcode { get; private set; }
        public string? SKU { get; private set; }
        public ItemType ItemType { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsStockable { get; private set; }
        public bool IsSerialized { get; private set; }
        public bool IsBatched { get; private set; }
        public decimal StandardCost { get; private set; }
        public decimal SellingPrice { get; private set; }
        public string? UnitOfMeasure { get; private set; }
        public decimal? MinimumStockLevel { get; private set; }
        public decimal? MaximumStockLevel { get; private set; }
        public decimal? ReorderPoint { get; private set; }
        public decimal? ReorderQuantity { get; private set; }
        public Guid? CategoryId { get; private set; }
        public Guid? BrandId { get; private set; }
        public Guid? SupplierId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }
        public List<ItemVariant> Variants { get; private set; }
        public List<ItemPriceHistory> PriceHistory { get; private set; }

        protected Item()
        {
            Variants = new List<ItemVariant>();
            PriceHistory = new List<ItemPriceHistory>();
        }

        public Item(string itemCode, string name, ItemType itemType, bool isStockable = true)
        {
            ItemCode = itemCode ?? throw new ArgumentNullException(nameof(itemCode));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ItemType = itemType;
            IsActive = true;
            IsStockable = isStockable;
            IsSerialized = false;
            IsBatched = false;
            StandardCost = 0;
            SellingPrice = 0;
            CreatedAt = DateTime.UtcNow;
            Variants = new List<ItemVariant>();
            PriceHistory = new List<ItemPriceHistory>();

            AddDomainEvent(new ItemCreatedDomainEvent(this));
        }

        public void UpdateBasicInfo(string name, string? description, string? barcode, string? sku)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Barcode = barcode;
            SKU = sku;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new ItemUpdatedDomainEvent(this));
        }

        public void SetPricing(decimal standardCost, decimal sellingPrice)
        {
            if (standardCost < 0)
                throw new TossErpDomainException("Standard cost cannot be negative.");
            if (sellingPrice < 0)
                throw new TossErpDomainException("Selling price cannot be negative.");

            StandardCost = standardCost;
            SellingPrice = sellingPrice;
            LastModifiedAt = DateTime.UtcNow;

            // Record price history
            var priceHistory = new ItemPriceHistory(sellingPrice, DateTime.UtcNow);
            PriceHistory.Add(priceHistory);

            AddDomainEvent(new ItemPricingUpdatedDomainEvent(this, standardCost, sellingPrice));
        }

        public void SetStockSettings(bool isSerialized, bool isBatched, string? unitOfMeasure, 
            decimal? minimumStockLevel, decimal? maximumStockLevel, decimal? reorderPoint, decimal? reorderQuantity)
        {
            IsSerialized = isSerialized;
            IsBatched = isBatched;
            UnitOfMeasure = unitOfMeasure;
            MinimumStockLevel = minimumStockLevel;
            MaximumStockLevel = maximumStockLevel;
            ReorderPoint = reorderPoint;
            ReorderQuantity = reorderQuantity;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new ItemStockSettingsUpdatedDomainEvent(this));
        }

        public void SetCategory(Guid? categoryId)
        {
            CategoryId = categoryId;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new ItemCategoryUpdatedDomainEvent(this, categoryId));
        }

        public void SetBrand(Guid? brandId)
        {
            BrandId = brandId;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new ItemBrandUpdatedDomainEvent(this, brandId));
        }

        public void SetSupplier(Guid? supplierId)
        {
            SupplierId = supplierId;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new ItemSupplierUpdatedDomainEvent(this, supplierId));
        }

        public void Activate()
        {
            if (IsActive)
                throw new TossErpDomainException("Item is already active.");

            IsActive = true;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new ItemActivatedDomainEvent(this));
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new TossErpDomainException("Item is already deactivated.");

            IsActive = false;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new ItemDeactivatedDomainEvent(this));
        }

        public void AddVariant(string variantName, string? variantCode = null)
        {
            if (string.IsNullOrWhiteSpace(variantName))
                throw new ArgumentException("Variant name cannot be null or empty.", nameof(variantName));

            if (Variants.Any(v => v.Name == variantName))
                throw new TossErpDomainException($"Variant '{variantName}' already exists.");

            var variant = new ItemVariant(variantName, variantCode);
            Variants.Add(variant);

            AddDomainEvent(new ItemVariantAddedDomainEvent(this, variant));
        }

        public void RemoveVariant(Guid variantId)
        {
            var variant = Variants.FirstOrDefault(v => v.Id == variantId);
            if (variant == null)
                throw new TossErpDomainException($"Variant with ID '{variantId}' not found.");

            Variants.Remove(variant);

            AddDomainEvent(new ItemVariantRemovedDomainEvent(this, variant));
        }

        public decimal GetCurrentPrice()
        {
            return PriceHistory.OrderByDescending(ph => ph.EffectiveDate).FirstOrDefault()?.Price ?? SellingPrice;
        }

        public bool NeedsReorder()
        {
            if (!IsStockable || ReorderPoint == null)
                return false;

            // This would typically check actual stock levels
            // For now, we'll return false as stock tracking is handled separately
            return false;
        }
    }
} 
