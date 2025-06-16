using POS.Domain.Common;
using POS.Domain.Models;

namespace POS.Domain.AggregatesModel.InventoryAggregate
{
    public class Inventory : Entity
    {
        public Guid StoreId { get; private set; }
        public int ProductId { get; private set; }
        public int CurrentStock { get; private set; }
        public int MinimumStock { get; private set; }
        public int MaximumStock { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }
        public List<StockMovement> Movements { get; private set; }

        private Inventory() { } // For EF Core

        public Inventory(
            Guid storeId,
            int productId,
            int currentStock,
            int minimumStock,
            int maximumStock)
        {
            StoreId = storeId;
            ProductId = productId;
            CurrentStock = currentStock;
            MinimumStock = minimumStock;
            MaximumStock = maximumStock;
            CreatedAt = DateTime.UtcNow;
            Movements = new List<StockMovement>();
        }

        public void AdjustStock(int quantity, string reason, string? reference = null)
        {
            if (CurrentStock + quantity < 0)
                throw new InvalidOperationException("Cannot reduce stock below zero");

            CurrentStock += quantity;
            LastModifiedAt = DateTime.UtcNow;

            var movement = new StockMovement(
                Id,
                quantity,
                reason,
                reference);
            Movements.Add(movement);
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
        }

        public bool IsLowStock() => CurrentStock <= MinimumStock;
        public bool IsOverstocked() => MaximumStock > 0 && CurrentStock >= MaximumStock;
    }

    public class StockMovement
    {
        public Guid InventoryId { get; private set; }
        public int Quantity { get; private set; }
        public string Reason { get; private set; }
        public string? Reference { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private StockMovement() { } // For EF Core

        public StockMovement(
            Guid inventoryId,
            int quantity,
            string reason,
            string? reference = null)
        {
            InventoryId = inventoryId;
            Quantity = quantity;
            Reason = reason;
            Reference = reference;
            CreatedAt = DateTime.UtcNow;
        }
    }
} 
