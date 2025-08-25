using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Infrastructure.Persistence.Configurations;

public class CustomerInteractionConfiguration : IEntityTypeConfiguration<CustomerInteraction>
{
    public void Configure(EntityTypeBuilder<CustomerInteraction> builder)
    {
        // Table configuration
        // builder.ToTable("CustomerInteractions"); // Remove for InMemory

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
            .HasMaxLength(2000);

        builder.Property(ci => ci.CreatedAt)
            .IsRequired();

        builder.Property(ci => ci.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ci => ci.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(ci => ci.FollowUpDate);

        // Indexes (simplified for InMemory)
        builder.HasIndex(ci => ci.CustomerId);
        builder.HasIndex(ci => ci.Type);
        builder.HasIndex(ci => ci.Status);
        builder.HasIndex(ci => ci.CreatedAt);
        builder.HasIndex(ci => ci.FollowUpDate);
        builder.HasIndex(ci => ci.CreatedBy);
    }
}
