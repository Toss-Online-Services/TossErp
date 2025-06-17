using POS.Domain.Common;
using POS.Domain.Exceptions;

namespace POS.Domain.AggregatesModel.InventoryAggregate
{
    public class InventoryAdjustment : Entity
    {
        public int PreviousQuantity { get; private set; }
        public int NewQuantity { get; private set; }
        public int AdjustmentQuantity => NewQuantity - PreviousQuantity;
        public string Reason { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string? ApprovedBy { get; private set; }
        public DateTime? ApprovedAt { get; private set; }
        public bool IsApproved => ApprovedAt.HasValue;

        private InventoryAdjustment()
        {
            PreviousQuantity = 0;
            NewQuantity = 0;
            Reason = string.Empty;
            CreatedAt = DateTime.UtcNow;
        }

        public InventoryAdjustment(int previousQuantity, int newQuantity, string reason)
        {
            if (previousQuantity < 0)
                throw new DomainException("Previous quantity cannot be negative");
            if (newQuantity < 0)
                throw new DomainException("New quantity cannot be negative");
            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("Adjustment reason cannot be empty");

            Id = Guid.NewGuid();
            PreviousQuantity = previousQuantity;
            NewQuantity = newQuantity;
            Reason = reason;
            CreatedAt = DateTime.UtcNow;
        }

        public void Approve(string approvedBy)
        {
            if (IsApproved)
                throw new DomainException("Adjustment is already approved");
            if (string.IsNullOrWhiteSpace(approvedBy))
                throw new DomainException("Approver cannot be empty");

            ApprovedBy = approvedBy;
            ApprovedAt = DateTime.UtcNow;
        }
    }
} 
