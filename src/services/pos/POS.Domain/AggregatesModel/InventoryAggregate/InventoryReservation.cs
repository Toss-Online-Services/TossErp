using POS.Domain.Common;
using POS.Domain.Exceptions;

namespace POS.Domain.AggregatesModel.InventoryAggregate
{
    public class InventoryReservation : Entity
    {
        public int Quantity { get; private set; }
        public string Reason { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ReleasedAt { get; private set; }
        public bool IsReleased => ReleasedAt.HasValue;

        private InventoryReservation()
        {
            Quantity = 0;
            Reason = string.Empty;
            CreatedAt = DateTime.UtcNow;
        }

        public InventoryReservation(int quantity, string reason)
        {
            if (quantity <= 0)
                throw new DomainException("Reservation quantity must be greater than zero");
            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("Reservation reason cannot be empty");

            Id = Guid.NewGuid();
            Quantity = quantity;
            Reason = reason;
            CreatedAt = DateTime.UtcNow;
        }

        public void Release()
        {
            if (IsReleased)
                throw new DomainException("Reservation is already released");

            ReleasedAt = DateTime.UtcNow;
        }
    }
} 
