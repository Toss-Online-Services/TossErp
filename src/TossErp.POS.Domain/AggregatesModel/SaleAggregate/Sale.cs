using System;
using System.Collections.Generic;
using System.Linq;
using TossErp.Domain.SeedWork;
using TossErp.POS.Domain.Events;
using TossErp.POS.Domain.Enums;
using TossErp.Domain.Exceptions;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate
{
    public class Sale : Entity, IAggregateRoot
    {
        public string SaleNumber { get; private set; } = string.Empty;
        public Guid CustomerId { get; private set; }
        public Guid CashierId { get; private set; }
        public SaleStatus Status { get; private set; }
        public SaleType SaleType { get; private set; }
        public decimal SubTotal { get; private set; }
        public decimal TaxAmount { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public decimal TotalAmount { get; private set; }
        public decimal AmountPaid { get; private set; }
        public decimal ChangeAmount { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public string? ReferenceNumber { get; private set; }
        public string? Notes { get; private set; }
        public DateTime SaleDate { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public DateTime? CancelledAt { get; private set; }
        public string? CancellationReason { get; private set; }
        public List<SaleItem> SaleItems { get; private set; }
        public List<SalePayment> Payments { get; private set; }

        protected Sale()
        {
            SaleItems = new List<SaleItem>();
            Payments = new List<SalePayment>();
        }

        public Sale(string saleNumber, Guid customerId, Guid cashierId, SaleType saleType = SaleType.Retail)
        {
            SaleNumber = saleNumber ?? throw new ArgumentNullException(nameof(saleNumber));
            CustomerId = customerId;
            CashierId = cashierId;
            SaleType = saleType;
            Status = SaleStatus.Draft;
            SubTotal = 0;
            TaxAmount = 0;
            DiscountAmount = 0;
            TotalAmount = 0;
            AmountPaid = 0;
            ChangeAmount = 0;
            SaleDate = DateTime.UtcNow;
            SaleItems = new List<SaleItem>();
            Payments = new List<SalePayment>();

            AddDomainEvent(new SaleCreatedDomainEvent(this));
        }

        public void AddItem(Guid itemId, string itemName, decimal quantity, decimal unitPrice, decimal? discountPercent = null)
        {
            if (Status != SaleStatus.Draft)
                throw new TossErpDomainException("Cannot add items to a sale that is not in draft status.");

            if (quantity <= 0)
                throw new TossErpDomainException("Quantity must be greater than zero.");

            if (unitPrice < 0)
                throw new TossErpDomainException("Unit price cannot be negative.");

            var existingItem = SaleItems.FirstOrDefault(si => si.ItemId == itemId);
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                var saleItem = new SaleItem(itemId, itemName, quantity, unitPrice, discountPercent);
                SaleItems.Add(saleItem);
            }

            RecalculateTotals();
            AddDomainEvent(new SaleItemAddedDomainEvent(this, itemId, quantity));
        }

        public void UpdateItemQuantity(Guid itemId, decimal newQuantity)
        {
            if (Status != SaleStatus.Draft)
                throw new TossErpDomainException("Cannot update items in a sale that is not in draft status.");

            var saleItem = SaleItems.FirstOrDefault(si => si.ItemId == itemId);
            if (saleItem == null)
                throw new TossErpDomainException($"Item with ID '{itemId}' not found in sale.");

            saleItem.UpdateQuantity(newQuantity);
            RecalculateTotals();
            AddDomainEvent(new SaleItemUpdatedDomainEvent(this, itemId, newQuantity));
        }

        public void RemoveItem(Guid itemId)
        {
            if (Status != SaleStatus.Draft)
                throw new TossErpDomainException("Cannot remove items from a sale that is not in draft status.");

            var saleItem = SaleItems.FirstOrDefault(si => si.ItemId == itemId);
            if (saleItem == null)
                throw new TossErpDomainException($"Item with ID '{itemId}' not found in sale.");

            SaleItems.Remove(saleItem);
            RecalculateTotals();
            AddDomainEvent(new SaleItemRemovedDomainEvent(this, itemId));
        }

        public void ApplyDiscount(decimal discountAmount)
        {
            if (Status != SaleStatus.Draft)
                throw new TossErpDomainException("Cannot apply discount to a sale that is not in draft status.");

            if (discountAmount < 0)
                throw new TossErpDomainException("Discount amount cannot be negative.");

            if (discountAmount > SubTotal)
                throw new TossErpDomainException("Discount amount cannot exceed subtotal.");

            DiscountAmount = discountAmount;
            RecalculateTotals();
            AddDomainEvent(new SaleDiscountAppliedDomainEvent(this, discountAmount));
        }

        public void Complete(PaymentMethod paymentMethod, decimal amountPaid, string? referenceNumber = null)
        {
            if (Status != SaleStatus.Draft)
                throw new TossErpDomainException("Cannot complete a sale that is not in draft status.");

            if (SaleItems.Count == 0)
                throw new TossErpDomainException("Cannot complete a sale with no items.");

            if (amountPaid < TotalAmount)
                throw new TossErpDomainException("Amount paid must be at least equal to total amount.");

            PaymentMethod = paymentMethod;
            AmountPaid = amountPaid;
            ChangeAmount = amountPaid - TotalAmount;
            ReferenceNumber = referenceNumber;
            Status = SaleStatus.Completed;
            CompletedAt = DateTime.UtcNow;

            var payment = new SalePayment(paymentMethod, amountPaid, referenceNumber);
            Payments.Add(payment);

            AddDomainEvent(new SaleCompletedDomainEvent(this));
        }

        public void Cancel(string reason)
        {
            if (Status == SaleStatus.Completed)
                throw new TossErpDomainException("Cannot cancel a completed sale.");

            if (Status == SaleStatus.Cancelled)
                throw new TossErpDomainException("Sale is already cancelled.");

            Status = SaleStatus.Cancelled;
            CancelledAt = DateTime.UtcNow;
            CancellationReason = reason ?? throw new ArgumentNullException(nameof(reason));

            AddDomainEvent(new SaleCancelledDomainEvent(this, reason));
        }

        public void AddPayment(PaymentMethod paymentMethod, decimal amount, string? referenceNumber = null)
        {
            if (Status != SaleStatus.Completed)
                throw new TossErpDomainException("Can only add payments to completed sales.");

            var payment = new SalePayment(paymentMethod, amount, referenceNumber);
            Payments.Add(payment);
            AmountPaid += amount;

            AddDomainEvent(new SalePaymentAddedDomainEvent(this, payment));
        }

        private void RecalculateTotals()
        {
            SubTotal = SaleItems.Sum(si => si.TotalPrice);
            TaxAmount = SubTotal * 0.1m; // 10% tax rate - should be configurable
            TotalAmount = SubTotal + TaxAmount - DiscountAmount;
        }

        public bool IsFullyPaid => AmountPaid >= TotalAmount;
        public decimal RemainingBalance => Math.Max(0, TotalAmount - AmountPaid);
    }
} 
