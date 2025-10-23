using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.CRM;

namespace Toss.Infrastructure.Data.Configurations;

public class CustomerInteractionConfiguration : IEntityTypeConfiguration<CustomerInteraction>
{
    public void Configure(EntityTypeBuilder<CustomerInteraction> builder)
    {
        builder.Property(ci => ci.InteractionType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ci => ci.Subject)
            .HasMaxLength(300);

        builder.Property(ci => ci.Description)
            .HasMaxLength(2000);

        builder.Property(ci => ci.InteractionBy)
            .HasMaxLength(200);

        builder.HasIndex(ci => ci.InteractionDate);
        builder.HasIndex(ci => new { ci.CustomerId, ci.RequiresFollowUp });
    }
}

