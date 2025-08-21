using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Projects.Domain.Entities;

namespace TossErp.Projects.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for TimeEntry entity
/// </summary>
public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        // Table configuration
        builder.ToTable("TimeEntries", "projects");
        
        // Primary key
        builder.HasKey(te => te.Id);
        
        // Properties
        builder.Property(te => te.Id)
            .IsRequired();

        builder.Property(te => te.ProjectId)
            .IsRequired();

        builder.Property(te => te.TaskId);

        builder.Property(te => te.UserId)
            .IsRequired();

        builder.Property(te => te.UserName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(te => te.Date)
            .IsRequired();

        builder.Property(te => te.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(te => te.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(te => te.Description)
            .HasMaxLength(1000);

        builder.Property(te => te.IsBillable)
            .IsRequired();

        builder.Property(te => te.CreatedAt)
            .IsRequired();

        builder.Property(te => te.ModifiedBy)
            .HasMaxLength(255);

        builder.Property(te => te.ModifiedAt);

        // Complex types for EF Core 9
        builder.ComplexProperty(te => te.Duration, d =>
        {
            d.Property(x => x.Hours)
                .HasColumnName("DurationHours")
                .HasColumnType("decimal(8,2)")
                .IsRequired();
        });

        builder.ComplexProperty(te => te.BillableAmount, ba =>
        {
            ba.Property(x => x.Amount)
                .HasColumnName("BillableAmount")
                .HasColumnType("decimal(18,2)");
            
            ba.Property(x => x.Currency)
                .HasColumnName("BillableCurrency")
                .HasMaxLength(3);
        });

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(te => te.ProjectId)
            .HasDatabaseName("IX_TimeEntries_ProjectId")
            .HasFillFactor(90);

        builder.HasIndex(te => new { te.ProjectId, te.TaskId })
            .HasDatabaseName("IX_TimeEntries_ProjectId_TaskId")
            .HasFillFactor(90);

        builder.HasIndex(te => new { te.ProjectId, te.UserId })
            .HasDatabaseName("IX_TimeEntries_ProjectId_UserId")
            .HasFillFactor(90);

        builder.HasIndex(te => new { te.UserId, te.Date })
            .HasDatabaseName("IX_TimeEntries_UserId_Date")
            .HasFillFactor(90);

        builder.HasIndex(te => te.Date)
            .HasDatabaseName("IX_TimeEntries_Date")
            .HasFillFactor(90);

        builder.HasIndex(te => new { te.ProjectId, te.Date })
            .HasDatabaseName("IX_TimeEntries_ProjectId_Date")
            .HasFillFactor(90);

        // Ignore navigation properties
        builder.Ignore(te => te.DomainEvents);
    }
}
