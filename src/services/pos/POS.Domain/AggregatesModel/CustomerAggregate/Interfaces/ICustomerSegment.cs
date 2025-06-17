namespace POS.Domain.AggregatesModel.CustomerAggregate.Interfaces
{
    public interface ICustomerSegment
    {
        Guid Id { get; }
        string Name { get; }
        string Description { get; }
        string? Criteria { get; }
        decimal? MinimumSpend { get; }
        int? MinimumOrders { get; }
        DateTime? StartDate { get; }
        DateTime? EndDate { get; }
        bool IsActive { get; }
        string? Tags { get; }
        int CustomerCount { get; }
        DateTime CreatedAt { get; }
        DateTime? LastModifiedAt { get; }
        string CreatedBy { get; }
        string? LastModifiedBy { get; }
        bool IsExpired { get; }
        bool IsActiveAndValid { get; }

        void UpdateDetails(
            string name,
            string description,
            string modifiedBy,
            string? criteria = null,
            decimal? minimumSpend = null,
            int? minimumOrders = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? tags = null);

        void Deactivate(string modifiedBy);
        void Reactivate(string modifiedBy);
        void IncrementCustomerCount();
        void DecrementCustomerCount();
    }
} 
