using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Infrastructure.Persistence.Configurations;

public class OpportunityConfiguration : IEntityTypeConfiguration<Opportunity>
{
    public void Configure(EntityTypeBuilder<Opportunity> builder)
    {
        // Table configuration
        builder.ToTable("Opportunities");

        // Primary key
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .IsRequired()
            .ValueGeneratedNever();

        // Basic properties
        builder.Property(o => o.TenantId)
            .IsRequired();

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Description)
            .HasMaxLength(2000);

        builder.Property(o => o.CustomerId)
            .IsRequired();

        builder.Property(o => o.LeadId);

        // Value Objects - Map individual properties to avoid constructor binding issues
        builder.Ignore(o => o.Value);
        
        // Map Money EstimatedValue directly
        builder.Property<decimal>("EstimatedValueAmount")
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        
        builder.Property<string>("EstimatedValueCurrency")
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");
            
        builder.Property<decimal>("Probability")
            .IsRequired()
            .HasColumnType("decimal(5,2)");

        // ActualValue Money property - use NotMapped attribute in domain model
        builder.Property<decimal?>("ActualValueAmount")
            .HasColumnType("decimal(18,2)");
            
        builder.Property<string?>("ActualValueCurrency")
            .HasMaxLength(3);

        // Enum properties
        builder.Property(o => o.Stage)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(o => o.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(o => o.Priority)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(o => o.Source)
            .HasConversion<string>()
            .HasMaxLength(30);

        // Timeline
        builder.Property(o => o.ExpectedCloseDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(o => o.ActualCloseDate)
            .HasColumnType("date");

        // Audit properties
        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.ModifiedAt);

        builder.Property(o => o.ModifiedBy)
            .HasMaxLength(100);

        // Assignment and ownership
        builder.Property(o => o.AssignedTo)
            .HasMaxLength(100);

        builder.Property(o => o.SalesTeam)
            .HasMaxLength(100);

        // Tracking
        builder.Property(o => o.LastActivityDate);

        builder.Property(o => o.ContactAttempts)
            .HasDefaultValue(0);

        builder.Property(o => o.NextFollowUp);

        // Closure details
        builder.Property(o => o.CloseReason)
            .HasMaxLength(500);

        builder.Property(o => o.CompetitorName)
            .HasMaxLength(200);

        builder.Property(o => o.Remarks)
            .HasMaxLength(2000);

        // StageProgressDays is a stored field
        builder.Property(o => o.StageProgressDays);

        // Computed/calculated properties are marked with [NotMapped] in domain model
        // IsOverdue, IsDeleted are stored fields

        builder.Property(o => o.IsDeleted)
            .HasDefaultValue(false);

        // Ignore computed properties
        builder.Ignore(o => o.WeightedValue);
        builder.Ignore(o => o.IsOpen);
        builder.Ignore(o => o.IsClosed);
        builder.Ignore(o => o.IsWon);
        builder.Ignore(o => o.IsLost);
        builder.Ignore(o => o.DaysInPipeline);
        builder.Ignore(o => o.DaysSinceLastActivity);
        builder.Ignore(o => o.DaysUntilExpectedClose);
        builder.Ignore(o => o.DaysToClose);
        builder.Ignore(o => o.TimeSinceLastActivity);
        builder.Ignore(o => o.IsStale);
        builder.Ignore(o => o.IsHighPriority);
        builder.Ignore(o => o.IsClosingSoon);
        builder.Ignore(o => o.IsOverdue);

        // Ignore collections for now (will be configured separately if needed)
        builder.Ignore(o => o.Activities);
        builder.Ignore(o => o.Notes);
        builder.Ignore(o => o.Communications);
        builder.Ignore(o => o.Documents);

        // Indexes
        builder.HasIndex(o => o.TenantId)
            .HasDatabaseName("IX_Opportunity_TenantId");

        builder.HasIndex(o => o.CustomerId)
            .HasDatabaseName("IX_Opportunity_CustomerId");

        builder.HasIndex(o => o.LeadId)
            .HasDatabaseName("IX_Opportunity_LeadId");

        builder.HasIndex(o => o.Stage)
            .HasDatabaseName("IX_Opportunity_Stage");

        builder.HasIndex(o => o.Type)
            .HasDatabaseName("IX_Opportunity_Type");

        builder.HasIndex(o => o.Priority)
            .HasDatabaseName("IX_Opportunity_Priority");

        builder.HasIndex(o => o.Source)
            .HasDatabaseName("IX_Opportunity_Source");

        builder.HasIndex(o => o.AssignedTo)
            .HasDatabaseName("IX_Opportunity_AssignedTo");

        builder.HasIndex(o => o.ExpectedCloseDate)
            .HasDatabaseName("IX_Opportunity_ExpectedCloseDate");

        builder.HasIndex(o => o.ActualCloseDate)
            .HasDatabaseName("IX_Opportunity_ActualCloseDate");

        builder.HasIndex(o => o.CreatedAt)
            .HasDatabaseName("IX_Opportunity_CreatedAt");

        builder.HasIndex(o => o.LastActivityDate)
            .HasDatabaseName("IX_Opportunity_LastActivityDate");

        builder.HasIndex(o => o.IsDeleted)
            .HasDatabaseName("IX_Opportunity_IsDeleted");

        // Composite indexes
        builder.HasIndex(o => new { o.TenantId, o.Stage })
            .HasDatabaseName("IX_Opportunity_Tenant_Stage");

        builder.HasIndex(o => new { o.TenantId, o.AssignedTo })
            .HasDatabaseName("IX_Opportunity_Tenant_AssignedTo");

        builder.HasIndex(o => new { o.CustomerId, o.Stage })
            .HasDatabaseName("IX_Opportunity_Customer_Stage");
    }
}
