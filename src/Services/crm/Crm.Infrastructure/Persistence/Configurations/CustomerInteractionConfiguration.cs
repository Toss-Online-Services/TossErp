using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Infrastructure.Persistence.Configurations;

public class CustomerInteractionConfiguration : IEntityTypeConfiguration<CustomerInteraction>
{
    public void Configure(EntityTypeBuilder<CustomerInteraction> builder)
    {
        // Table configuration
        builder.ToTable("CustomerInteractions");

        // Primary key
        builder.HasKey(ci => ci.Id);
        builder.Property(ci => ci.Id)
            .IsRequired()
            .ValueGeneratedNever();

        // Foreign key
        builder.Property(ci => ci.CustomerId)
            .IsRequired();

        // Properties
        builder.Property(ci => ci.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(ci => ci.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(ci => ci.Notes)
            .HasMaxLength(2000)
            .HasDefaultValue(string.Empty);

        builder.Property(ci => ci.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(ci => ci.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ci => ci.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(ci => ci.FollowUpDate)
            .HasColumnType("timestamp with time zone");

        // Indexes
        builder.HasIndex(ci => ci.CustomerId)
            .HasDatabaseName("IX_CustomerInteraction_CustomerId");

        builder.HasIndex(ci => ci.Type)
            .HasDatabaseName("IX_CustomerInteraction_Type");

        builder.HasIndex(ci => ci.Status)
            .HasDatabaseName("IX_CustomerInteraction_Status");

        builder.HasIndex(ci => ci.CreatedAt)
            .HasDatabaseName("IX_CustomerInteraction_CreatedAt");

        builder.HasIndex(ci => ci.FollowUpDate)
            .HasDatabaseName("IX_CustomerInteraction_FollowUpDate");

        builder.HasIndex(ci => ci.CreatedBy)
            .HasDatabaseName("IX_CustomerInteraction_CreatedBy");
    }
}
