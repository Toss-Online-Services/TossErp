using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerEnrolledInLoyaltyProgramDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public Guid LoyaltyProgramId { get; }
    public string ProgramName { get; }
    public string MembershipNumber { get; }
    public string MembershipTier { get; }
    public string EnrolledBy { get; }
    public DateTime EnrolledAt { get; }

    public CustomerEnrolledInLoyaltyProgramDomainEvent(
        Guid customerId,
        Guid loyaltyProgramId,
        string programName,
        string membershipNumber,
        string membershipTier,
        string enrolledBy,
        DateTime enrolledAt)
    {
        CustomerId = customerId;
        LoyaltyProgramId = loyaltyProgramId;
        ProgramName = programName;
        MembershipNumber = membershipNumber;
        MembershipTier = membershipTier;
        EnrolledBy = enrolledBy;
        EnrolledAt = enrolledAt;
    }
} 
