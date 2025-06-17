using POS.Domain.Common;
using POS.Domain.Common.Events;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Exceptions;
using POS.Domain.SeedWork;
using POS.Domain.AggregatesModel.InventoryAggregate.Events;

namespace POS.Domain.AggregatesModel.InventoryAggregate
{
    public class Inventory : AggregateRoot
    {
        public Guid StoreId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public string Reason { get; private set; }
        public int CurrentStock { get; private set; }
        public int MinimumStock { get; private set; }
        public int MaximumStock { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }
        public List<InventoryMovement> Movements { get; private set; }

        private Inventory()
        {
            Reason = string.Empty;
            Movements = new List<InventoryMovement>();
        }

        public Inventory(Guid storeId, Guid productId, int quantity, string reason) : this()
        {
            StoreId = storeId;
            ProductId = productId;
            Quantity = quantity;
            Reason = reason ?? string.Empty;
            CreatedAt = DateTime.UtcNow;
            AddDomainEvent(new InventoryCreatedDomainEvent(Id, productId, storeId));
        }

        public void AddMovement(int quantity, string reason)
        {
            var movement = new InventoryMovement(quantity, reason);
            Movements.Add(movement);
            Quantity += quantity;

            AddDomainEvent(new InventoryMovementAddedDomainEvent(Id, movement.Id, DateTime.UtcNow));
        }

        public void UpdateQuantity(int newQuantity, string reason)
        {
            if (newQuantity < 0)
                throw new DomainException("Quantity cannot be negative");

            var movement = new InventoryMovement(newQuantity - Quantity, reason);
            Movements.Add(movement);
            Quantity = newQuantity;

            AddDomainEvent(new InventoryUpdatedDomainEvent(Id, DateTime.UtcNow));
        }

        public void UpdateStockLevels(int minimumStock, int maximumStock)
        {
            if (minimumStock < 0)
                throw new ArgumentException("Minimum stock cannot be negative", nameof(minimumStock));
            if (maximumStock < 0)
                throw new ArgumentException("Maximum stock cannot be negative", nameof(maximumStock));
            if (maximumStock > 0 && minimumStock > maximumStock)
                throw new ArgumentException("Minimum stock cannot be greater than maximum stock");

            MinimumStock = minimumStock;
            MaximumStock = maximumStock;
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new InventoryUpdatedDomainEvent(Id, DateTime.UtcNow));
        }

        public bool IsLowStock() => CurrentStock <= MinimumStock;
        public bool IsOverstocked() => MaximumStock > 0 && CurrentStock >= MaximumStock;
    }

    public class InventoryMovement : Entity
    {
        public int Quantity { get; private set; }
        public string Reason { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private InventoryMovement() 
        {
            Reason = string.Empty;
        }

        public InventoryMovement(int quantity, string reason)
        {
            Quantity = quantity;
            Reason = reason ?? string.Empty;
            CreatedAt = DateTime.UtcNow;
        }
    }
} 
