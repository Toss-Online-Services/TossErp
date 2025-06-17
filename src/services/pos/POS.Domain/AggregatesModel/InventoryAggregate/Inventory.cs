using POS.Domain.Common;
using POS.Domain.Common.Events;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Exceptions;
using POS.Domain.SeedWork;
using POS.Domain.AggregatesModel.InventoryAggregate.Events;
using POS.Domain.Enums;

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

        // New properties for advanced inventory management
        public string? LotNumber { get; private set; }
        public DateTime? ExpiryDate { get; private set; }
        public string? SerialNumber { get; private set; }
        public UnitOfMeasure UnitOfMeasure { get; private set; }
        public decimal UnitCost { get; private set; }
        public string? Location { get; private set; }
        public string? BinNumber { get; private set; }
        public bool IsActive { get; private set; }
        public List<InventoryReservation> Reservations { get; private set; }
        public List<InventoryAdjustment> Adjustments { get; private set; }

        private Inventory()
        {
            Reason = string.Empty;
            Movements = new List<InventoryMovement>();
            Reservations = new List<InventoryReservation>();
            Adjustments = new List<InventoryAdjustment>();
            IsActive = true;
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
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new InventoryMovementAddedDomainEvent(Id, movement.Id, DateTime.UtcNow));
        }

        public void UpdateQuantity(int newQuantity, string reason)
        {
            if (newQuantity < 0)
                throw new DomainException("Quantity cannot be negative");

            var adjustment = new InventoryAdjustment(Quantity, newQuantity, reason);
            Adjustments.Add(adjustment);
            Quantity = newQuantity;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new InventoryQuantityUpdatedDomainEvent(Id, Quantity, reason));
        }

        public void SetLotNumber(string lotNumber)
        {
            if (string.IsNullOrWhiteSpace(lotNumber))
                throw new DomainException("Lot number cannot be empty");

            LotNumber = lotNumber;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new InventoryLotNumberUpdatedDomainEvent(Id, lotNumber));
        }

        public void SetExpiryDate(DateTime expiryDate)
        {
            if (expiryDate <= DateTime.UtcNow)
                throw new DomainException("Expiry date must be in the future");

            ExpiryDate = expiryDate;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new InventoryExpiryDateUpdatedDomainEvent(Id, expiryDate));
        }

        public void SetSerialNumber(string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(serialNumber))
                throw new DomainException("Serial number cannot be empty");

            SerialNumber = serialNumber;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new InventorySerialNumberUpdatedDomainEvent(Id, serialNumber));
        }

        public void SetUnitOfMeasure(UnitOfMeasure unitOfMeasure)
        {
            UnitOfMeasure = unitOfMeasure;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new InventoryUnitOfMeasureUpdatedDomainEvent(Id, unitOfMeasure));
        }

        public void UpdateUnitCost(decimal unitCost)
        {
            if (unitCost < 0)
                throw new DomainException("Unit cost cannot be negative");

            UnitCost = unitCost;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new InventoryUnitCostUpdatedDomainEvent(Id, unitCost));
        }

        public void SetLocation(string location, string binNumber)
        {
            if (string.IsNullOrWhiteSpace(location))
                throw new DomainException("Location cannot be empty");

            Location = location;
            BinNumber = binNumber;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new InventoryLocationUpdatedDomainEvent(Id, location, binNumber));
        }

        public void Reserve(int quantity, string reason)
        {
            if (quantity <= 0)
                throw new DomainException("Reservation quantity must be greater than zero");

            if (quantity > AvailableQuantity)
                throw new DomainException("Insufficient quantity available for reservation");

            var reservation = new InventoryReservation(quantity, reason);
            Reservations.Add(reservation);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new InventoryReservedDomainEvent(Id, quantity, reason));
        }

        public void ReleaseReservation(Guid reservationId)
        {
            var reservation = Reservations.FirstOrDefault(r => r.Id == reservationId);
            if (reservation == null)
                throw new DomainException("Reservation not found");

            Reservations.Remove(reservation);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new InventoryReservationReleasedDomainEvent(Id, reservationId));
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException("Inventory is already inactive");

            IsActive = false;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new InventoryDeactivatedDomainEvent(Id));
        }

        public void Reactivate()
        {
            if (IsActive)
                throw new DomainException("Inventory is already active");

            IsActive = true;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new InventoryReactivatedDomainEvent(Id));
        }

        public int AvailableQuantity => Quantity - Reservations.Sum(r => r.Quantity);
        public bool IsLowStock => CurrentStock <= MinimumStock;
        public bool IsOverstocked => CurrentStock >= MaximumStock;
        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value <= DateTime.UtcNow;
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
