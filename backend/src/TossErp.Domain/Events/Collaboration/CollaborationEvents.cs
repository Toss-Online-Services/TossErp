using TossErp.Domain.Common;

namespace TossErp.Domain.Events.Collaboration;

// Buying Group Events
public class BuyingGroupActivated : DomainEvent
{
    public int BuyingGroupId { get; }
    public string GroupNumber { get; }
    public int MemberCount { get; }
    
    public BuyingGroupActivated(int buyingGroupId, string groupNumber, int memberCount)
    {
        BuyingGroupId = buyingGroupId;
        GroupNumber = groupNumber;
        MemberCount = memberCount;
    }
}

public class MemberJoinedGroup : DomainEvent
{
    public int BuyingGroupId { get; }
    public int CustomerId { get; }
    public string CustomerName { get; }
    public decimal Commitment { get; }
    
    public MemberJoinedGroup(int buyingGroupId, int customerId, string customerName, decimal commitment)
    {
        BuyingGroupId = buyingGroupId;
        CustomerId = customerId;
        CustomerName = customerName;
        Commitment = commitment;
    }
}

public class BuyingGroupClosed : DomainEvent
{
    public int BuyingGroupId { get; }
    public string GroupNumber { get; }
    public decimal TotalPurchaseValue { get; }
    public decimal TotalSavings { get; }
    
    public BuyingGroupClosed(int buyingGroupId, string groupNumber, decimal totalPurchaseValue, decimal totalSavings)
    {
        BuyingGroupId = buyingGroupId;
        GroupNumber = groupNumber;
        TotalPurchaseValue = totalPurchaseValue;
        TotalSavings = totalSavings;
    }
}

// Shared Logistics Events
public class DeliveryPoolCreated : DomainEvent
{
    public int DeliveryPoolId { get; }
    public string PoolNumber { get; }
    public int ParticipantCount { get; }
    
    public DeliveryPoolCreated(int deliveryPoolId, string poolNumber, int participantCount)
    {
        DeliveryPoolId = deliveryPoolId;
        PoolNumber = poolNumber;
        ParticipantCount = participantCount;
    }
}

// Asset Sharing Events
public class AssetListedForRent : DomainEvent
{
    public int AssetId { get; }
    public string AssetName { get; }
    public int OwnerId { get; }
    public decimal DailyRate { get; }
    
    public AssetListedForRent(int assetId, string assetName, int ownerId, decimal dailyRate)
    {
        AssetId = assetId;
        AssetName = assetName;
        OwnerId = ownerId;
        DailyRate = dailyRate;
    }
}

public class AssetRented : DomainEvent
{
    public int RentalId { get; }
    public int AssetId { get; }
    public int RenterId { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    
    public AssetRented(int rentalId, int assetId, int renterId, DateTime startDate, DateTime endDate)
    {
        RentalId = rentalId;
        AssetId = assetId;
        RenterId = renterId;
        StartDate = startDate;
        EndDate = endDate;
    }
}

// Credit Pool Events
public class CreditPoolCreated : DomainEvent
{
    public int CreditPoolId { get; }
    public string PoolName { get; }
    public decimal TotalFund { get; }
    
    public CreditPoolCreated(int creditPoolId, string poolName, decimal totalFund)
    {
        CreditPoolId = creditPoolId;
        PoolName = poolName;
        TotalFund = totalFund;
    }
}

public class CreditAllocated : DomainEvent
{
    public int AllocationId { get; }
    public int CreditPoolId { get; }
    public int BorrowerId { get; }
    public decimal Amount { get; }
    
    public CreditAllocated(int allocationId, int creditPoolId, int borrowerId, decimal amount)
    {
        AllocationId = allocationId;
        CreditPoolId = creditPoolId;
        BorrowerId = borrowerId;
        Amount = amount;
    }
}

