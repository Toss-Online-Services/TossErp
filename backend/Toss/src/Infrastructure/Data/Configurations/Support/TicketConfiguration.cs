using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Support;
using Toss.Domain.Enums;

namespace Toss.Infrastructure.Data.Configurations.Support;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.LinkedEntityType)
            .HasMaxLength(100);

        builder.Property(t => t.CreatedById)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(t => t.AssignedToId)
            .HasMaxLength(450);

        builder.Property(t => t.Type)
            .HasConversion<int>();

        builder.Property(t => t.Status)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(t => t.Business)
            .WithMany()
            .HasForeignKey(t => t.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Notes)
            .WithOne(n => n.Ticket)
            .HasForeignKey(n => n.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(t => t.BusinessId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.Type);
        builder.HasIndex(t => t.CreatedById);
        builder.HasIndex(t => t.AssignedToId);
        builder.HasIndex(t => new { t.LinkedEntityType, t.LinkedEntityId });
    }
}

