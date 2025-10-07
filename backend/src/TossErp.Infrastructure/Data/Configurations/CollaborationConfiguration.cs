using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Domain.Entities.Collaboration;

namespace TossErp.Infrastructure.Data.Configurations;

// Group Buying Configurations
public class BuyingGroupConfiguration : IEntityTypeConfiguration<BuyingGroup>
{
    public void Configure(EntityTypeBuilder<BuyingGroup> builder)
    {
        builder.ToTable("BuyingGroups");
        
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.GroupNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(b => b.GroupNumber).IsUnique();
        
        builder.Property(b => b.Name).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(b => b.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(b => b.OrganizerName).IsRequired().HasMaxLength(200);
        
        builder.Property(b => b.MinimumOrderValue).HasPrecision(18, 2);
        builder.Property(b => b.TargetDiscount).HasPrecision(5, 2);
        builder.Property(b => b.TotalPurchaseValue).HasPrecision(18, 2);
        builder.Property(b => b.TotalSavings).HasPrecision(18, 2);
        builder.Property(b => b.MembershipFee).HasPrecision(18, 2);
        
        builder.HasIndex(b => b.Status);
        builder.HasIndex(b => b.OrganizerId);
        
        builder.HasMany(b => b.Members).WithOne(m => m.BuyingGroup).HasForeignKey(m => m.BuyingGroupId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(b => b.PurchaseOrders).WithOne(p => p.BuyingGroup).HasForeignKey(p => p.BuyingGroupId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        builder.ToTable("GroupMembers");
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.CustomerName).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(m => m.CommitmentAmount).HasPrecision(18, 2);
        builder.Property(m => m.ActualPurchaseAmount).HasPrecision(18, 2);
        builder.Property(m => m.SavingsAmount).HasPrecision(18, 2);
        
        builder.HasIndex(m => m.BuyingGroupId);
        builder.HasIndex(m => m.CustomerId);
    }
}

public class GroupPurchaseOrderConfiguration : IEntityTypeConfiguration<GroupPurchaseOrder>
{
    public void Configure(EntityTypeBuilder<GroupPurchaseOrder> builder)
    {
        builder.ToTable("GroupPurchaseOrders");
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.OrderNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(p => p.OrderNumber).IsUnique();
        
        builder.Property(p => p.SupplierName).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(p => p.TotalAmount).HasPrecision(18, 2);
        builder.Property(p => p.DiscountPercentage).HasPrecision(5, 2);
        builder.Property(p => p.DiscountAmount).HasPrecision(18, 2);
        builder.Property(p => p.NetAmount).HasPrecision(18, 2);
        builder.Property(p => p.ShippingCost).HasPrecision(18, 2);
        builder.Property(p => p.GrandTotal).HasPrecision(18, 2);
        builder.Property(p => p.SharedCostPerMember).HasPrecision(18, 2);
        builder.Property(p => p.TotalPaid).HasPrecision(18, 2);
        builder.Property(p => p.TotalDue).HasPrecision(18, 2);
        
        builder.HasIndex(p => p.BuyingGroupId);
        builder.HasIndex(p => p.Status);
        
        builder.HasMany(p => p.Items).WithOne(i => i.GroupPurchaseOrder).HasForeignKey(i => i.GroupPurchaseOrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.MemberAllocations).WithOne(m => m.GroupPurchaseOrder).HasForeignKey(m => m.GroupPurchaseOrderId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class GroupPurchaseOrderItemConfiguration : IEntityTypeConfiguration<GroupPurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<GroupPurchaseOrderItem> builder)
    {
        builder.ToTable("GroupPurchaseOrderItems");
        builder.HasKey(i => i.Id);
        
        builder.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(i => i.TotalQuantity).HasPrecision(18, 4);
        builder.Property(i => i.UnitPrice).HasPrecision(18, 2);
        builder.Property(i => i.DiscountPercentage).HasPrecision(5, 2);
        builder.Property(i => i.DiscountedPrice).HasPrecision(18, 2);
        builder.Property(i => i.LineTotal).HasPrecision(18, 2);
        
        builder.HasIndex(i => i.GroupPurchaseOrderId);
    }
}

public class MemberOrderAllocationConfiguration : IEntityTypeConfiguration<MemberOrderAllocation>
{
    public void Configure(EntityTypeBuilder<MemberOrderAllocation> builder)
    {
        builder.ToTable("MemberOrderAllocations");
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.CustomerName).IsRequired().HasMaxLength(200);
        builder.Property(m => m.AllocatedAmount).HasPrecision(18, 2);
        builder.Property(m => m.ShippingShare).HasPrecision(18, 2);
        builder.Property(m => m.TotalDue).HasPrecision(18, 2);
        builder.Property(m => m.PaidAmount).HasPrecision(18, 2);
        
        builder.HasIndex(m => m.GroupPurchaseOrderId);
        builder.HasIndex(m => m.CustomerId);
    }
}

// Shared Logistics Configurations
public class DeliveryPoolConfiguration : IEntityTypeConfiguration<DeliveryPool>
{
    public void Configure(EntityTypeBuilder<DeliveryPool> builder)
    {
        builder.ToTable("DeliveryPools");
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.PoolNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(d => d.PoolNumber).IsUnique();
        
        builder.Property(d => d.Name).IsRequired().HasMaxLength(200);
        builder.Property(d => d.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(d => d.OrganizerName).IsRequired().HasMaxLength(200);
        builder.Property(d => d.DistanceKm).HasPrecision(10, 2);
        builder.Property(d => d.MaximumWeight).HasPrecision(18, 2);
        builder.Property(d => d.MaximumVolume).HasPrecision(18, 4);
        builder.Property(d => d.EstimatedCost).HasPrecision(18, 2);
        builder.Property(d => d.ActualCost).HasPrecision(18, 2);
        builder.Property(d => d.CostPerParticipant).HasPrecision(18, 2);
        builder.Property(d => d.SavingsPerParticipant).HasPrecision(18, 2);
        
        builder.HasIndex(d => d.Status);
        builder.HasIndex(d => d.ScheduledDate);
        
        builder.Ignore(d => d.AvailableAmount);
        
        builder.HasMany(d => d.Participants).WithOne(p => p.DeliveryPool).HasForeignKey(p => p.DeliveryPoolId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(d => d.Stops).WithOne(s => s.DeliveryPool).HasForeignKey(s => s.DeliveryPoolId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class DeliveryPoolParticipantConfiguration : IEntityTypeConfiguration<DeliveryPoolParticipant>
{
    public void Configure(EntityTypeBuilder<DeliveryPoolParticipant> builder)
    {
        builder.ToTable("DeliveryPoolParticipants");
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.CustomerName).IsRequired().HasMaxLength(200);
        builder.Property(p => p.DeliveryAddress).IsRequired().HasMaxLength(500);
        builder.Property(p => p.Latitude).HasPrecision(10, 7);
        builder.Property(p => p.Longitude).HasPrecision(10, 7);
        builder.Property(p => p.TotalWeight).HasPrecision(18, 2);
        builder.Property(p => p.TotalVolume).HasPrecision(18, 4);
        builder.Property(p => p.CostShare).HasPrecision(18, 2);
        
        builder.HasIndex(p => p.DeliveryPoolId);
    }
}

public class PoolStopConfiguration : IEntityTypeConfiguration<PoolStop>
{
    public void Configure(EntityTypeBuilder<PoolStop> builder)
    {
        builder.ToTable("PoolStops");
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Address).IsRequired().HasMaxLength(500);
        builder.Property(s => s.Latitude).HasPrecision(10, 7);
        builder.Property(s => s.Longitude).HasPrecision(10, 7);
        
        builder.HasIndex(s => s.DeliveryPoolId);
        builder.HasIndex(s => s.Sequence);
    }
}

// Asset Sharing Configurations
public class SharedAssetConfiguration : IEntityTypeConfiguration<SharedAsset>
{
    public void Configure(EntityTypeBuilder<SharedAsset> builder)
    {
        builder.ToTable("SharedAssets");
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.AssetNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(a => a.AssetNumber).IsUnique();
        
        builder.Property(a => a.Name).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Category).HasConversion<string>().HasMaxLength(50);
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(a => a.OwnerName).IsRequired().HasMaxLength(200);
        
        builder.Property(a => a.HourlyRate).HasPrecision(18, 2);
        builder.Property(a => a.DailyRate).HasPrecision(18, 2);
        builder.Property(a => a.WeeklyRate).HasPrecision(18, 2);
        builder.Property(a => a.MonthlyRate).HasPrecision(18, 2);
        builder.Property(a => a.SecurityDeposit).HasPrecision(18, 2);
        builder.Property(a => a.TotalRevenue).HasPrecision(18, 2);
        builder.Property(a => a.AverageRating).HasPrecision(3, 2);
        builder.Property(a => a.Latitude).HasPrecision(10, 7);
        builder.Property(a => a.Longitude).HasPrecision(10, 7);
        
        builder.HasIndex(a => a.Category);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.IsAvailable);
        
        builder.HasMany(a => a.Rentals).WithOne(r => r.Asset).HasForeignKey(r => r.AssetId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class AssetRentalConfiguration : IEntityTypeConfiguration<AssetRental>
{
    public void Configure(EntityTypeBuilder<AssetRental> builder)
    {
        builder.ToTable("AssetRentals");
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.RentalNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(r => r.RentalNumber).IsUnique();
        
        builder.Property(r => r.AssetName).IsRequired().HasMaxLength(200);
        builder.Property(r => r.RenterName).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(50);
        
        builder.Property(r => r.ActualDurationHours).HasPrecision(10, 2);
        builder.Property(r => r.RatePerDay).HasPrecision(18, 2);
        builder.Property(r => r.SecurityDeposit).HasPrecision(18, 2);
        builder.Property(r => r.RentalAmount).HasPrecision(18, 2);
        builder.Property(r => r.LateFee).HasPrecision(18, 2);
        builder.Property(r => r.DamageFee).HasPrecision(18, 2);
        builder.Property(r => r.TotalAmount).HasPrecision(18, 2);
        
        builder.HasIndex(r => r.AssetId);
        builder.HasIndex(r => r.RenterId);
        builder.HasIndex(r => r.Status);
    }
}

// Credit Pool Configurations
public class CreditPoolConfiguration : IEntityTypeConfiguration<CreditPool>
{
    public void Configure(EntityTypeBuilder<CreditPool> builder)
    {
        builder.ToTable("CreditPools");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.PoolNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(c => c.PoolNumber).IsUnique();
        
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(c => c.AdministratorName).IsRequired().HasMaxLength(200);
        
        builder.Property(c => c.TotalFund).HasPrecision(18, 2);
        builder.Property(c => c.AllocatedAmount).HasPrecision(18, 2);
        builder.Property(c => c.OutstandingAmount).HasPrecision(18, 2);
        builder.Property(c => c.RepaidAmount).HasPrecision(18, 2);
        builder.Property(c => c.InterestRate).HasPrecision(5, 2);
        builder.Property(c => c.MaximumLoanAmount).HasPrecision(18, 2);
        builder.Property(c => c.MinimumLoanAmount).HasPrecision(18, 2);
        builder.Property(c => c.MembershipFee).HasPrecision(18, 2);
        builder.Property(c => c.MinimumContribution).HasPrecision(18, 2);
        builder.Property(c => c.DefaultRate).HasPrecision(5, 2);
        builder.Property(c => c.ReserveRequirement).HasPrecision(5, 2);
        builder.Property(c => c.ReserveFund).HasPrecision(18, 2);
        builder.Property(c => c.TotalLoansIssued).HasPrecision(18, 2);
        builder.Property(c => c.TotalInterestEarned).HasPrecision(18, 2);
        builder.Property(c => c.AverageRecoveryRate).HasPrecision(5, 2);
        
        builder.HasIndex(c => c.Status);
        
        builder.Ignore(c => c.AvailableAmount);
        
        builder.HasMany(c => c.Members).WithOne(m => m.CreditPool).HasForeignKey(m => m.CreditPoolId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(c => c.Allocations).WithOne(a => a.CreditPool).HasForeignKey(a => a.CreditPoolId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class CreditPoolMemberConfiguration : IEntityTypeConfiguration<CreditPoolMember>
{
    public void Configure(EntityTypeBuilder<CreditPoolMember> builder)
    {
        builder.ToTable("CreditPoolMembers");
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.CustomerName).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(m => m.ContributionAmount).HasPrecision(18, 2);
        builder.Property(m => m.MaximumBorrowingLimit).HasPrecision(18, 2);
        builder.Property(m => m.TotalBorrowed).HasPrecision(18, 2);
        builder.Property(m => m.TotalRepaid).HasPrecision(18, 2);
        builder.Property(m => m.CurrentOutstanding).HasPrecision(18, 2);
        
        builder.HasIndex(m => m.CreditPoolId);
        builder.HasIndex(m => m.CustomerId);
    }
}

public class CreditAllocationConfiguration : IEntityTypeConfiguration<CreditAllocation>
{
    public void Configure(EntityTypeBuilder<CreditAllocation> builder)
    {
        builder.ToTable("CreditAllocations");
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.AllocationNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(a => a.AllocationNumber).IsUnique();
        
        builder.Property(a => a.BorrowerName).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(a => a.Purpose).IsRequired().HasMaxLength(500);
        
        builder.Property(a => a.PrincipalAmount).HasPrecision(18, 2);
        builder.Property(a => a.InterestRate).HasPrecision(5, 2);
        builder.Property(a => a.MonthlyPayment).HasPrecision(18, 2);
        builder.Property(a => a.TotalRepayableAmount).HasPrecision(18, 2);
        builder.Property(a => a.AmountRepaid).HasPrecision(18, 2);
        builder.Property(a => a.OutstandingBalance).HasPrecision(18, 2);
        
        builder.HasIndex(a => a.CreditPoolId);
        builder.HasIndex(a => a.BorrowerId);
        builder.HasIndex(a => a.Status);
    }
}

// Community Features Configurations
public class BusinessDirectoryConfiguration : IEntityTypeConfiguration<BusinessDirectory>
{
    public void Configure(EntityTypeBuilder<BusinessDirectory> builder)
    {
        builder.ToTable("BusinessDirectory");
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.BusinessName).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Category).IsRequired().HasMaxLength(100);
        builder.Property(b => b.AverageRating).HasPrecision(3, 2);
        builder.Property(b => b.Latitude).HasPrecision(10, 7);
        builder.Property(b => b.Longitude).HasPrecision(10, 7);
        
        builder.HasIndex(b => b.CustomerId);
        builder.HasIndex(b => b.Category);
        builder.HasIndex(b => b.IsActive);
        builder.HasIndex(b => b.IsVerified);
    }
}

public class CommunityEventConfiguration : IEntityTypeConfiguration<CommunityEvent>
{
    public void Configure(EntityTypeBuilder<CommunityEvent> builder)
    {
        builder.ToTable("CommunityEvents");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.EventNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(e => e.EventNumber).IsUnique();
        
        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.OrganizerName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.RegistrationFee).HasPrecision(18, 2);
        
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.StartDateTime);
        builder.HasIndex(e => e.OrganizerId);
        
        builder.HasMany(e => e.Registrations).WithOne(r => r.Event).HasForeignKey(r => r.EventId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class EventRegistrationConfiguration : IEntityTypeConfiguration<EventRegistration>
{
    public void Configure(EntityTypeBuilder<EventRegistration> builder)
    {
        builder.ToTable("EventRegistrations");
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.CustomerName).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(r => r.FeeAmount).HasPrecision(18, 2);
        
        builder.HasIndex(r => r.EventId);
        builder.HasIndex(r => r.CustomerId);
        builder.HasIndex(r => r.Status);
    }
}

