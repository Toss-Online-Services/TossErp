using System;
using TossErp.Domain.SeedWork;
using TossErp.Domain.Exceptions;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleItem : Entity
    {
        public Guid ItemId { get; private set; }
        public string ItemName { get; private set; } = string.Empty;
        public string ItemCode { get; private set; } = string.Empty;
        public decimal Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal? DiscountPercent { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public decimal TotalPrice { get; private set; }

        // Alias properties for API compatibility
        public decimal? DiscountPercentage => DiscountPercent;
        public decimal TotalAmount => TotalPrice;

        protected SaleItem() { }

        public SaleItem(Guid itemId, string itemName, decimal quantity, decimal unitPrice, decimal? discountPercent = null)
        {
            ItemId = itemId;
            ItemName = itemName ?? throw new ArgumentNullException(nameof(itemName));
            ItemCode = string.Empty; // Will be set when item details are loaded
            Quantity = quantity;
            UnitPrice = unitPrice;
            DiscountPercent = discountPercent;
            CalculateTotals();
        }

        public SaleItem(Guid itemId, string itemName, string itemCode, decimal quantity, decimal unitPrice, decimal? discountPercent = null)
        {
            ItemId = itemId;
            ItemName = itemName ?? throw new ArgumentNullException(nameof(itemName));
            ItemCode = itemCode ?? throw new ArgumentNullException(nameof(itemCode));
            Quantity = quantity;
            UnitPrice = unitPrice;
            DiscountPercent = discountPercent;
            CalculateTotals();
        }

        public void UpdateQuantity(decimal newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(newQuantity));

            Quantity = newQuantity;
            CalculateTotals();
        }

        public void UpdateUnitPrice(decimal newUnitPrice)
        {
            if (newUnitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative.", nameof(newUnitPrice));

            UnitPrice = newUnitPrice;
            CalculateTotals();
        }

        public void ApplyDiscount(decimal discountPercent)
        {
            if (discountPercent < 0 || discountPercent > 100)
                throw new ArgumentException("Discount percent must be between 0 and 100.", nameof(discountPercent));

            DiscountPercent = discountPercent;
            CalculateTotals();
        }

        private void CalculateTotals()
        {
            var subtotal = Quantity * UnitPrice;
            DiscountAmount = DiscountPercent.HasValue ? subtotal * (DiscountPercent.Value / 100m) : 0;
            TotalPrice = subtotal - DiscountAmount;
        }
    }
} 
