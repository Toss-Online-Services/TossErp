using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Quality;
using Toss.Domain.Enums;

namespace Toss.Infrastructure.Data.Configurations.Quality;

public class ActionItemConfiguration : IEntityTypeConfiguration<ActionItem>
{
    public void Configure(EntityTypeBuilder<ActionItem> builder)
    {
        builder.Property(a => a.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(a => a.Description)
            .HasMaxLength(2000);

        builder.Property(a => a.Notes)
            .HasMaxLength(1000);

        builder.Property(a => a.AssignedToId)
            .HasMaxLength(450);

        builder.Property(a => a.Status)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(a => a.Business)
            .WithMany()
            .HasForeignKey(a => a.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Incident)
            .WithMany(i => i.ActionItems)
            .HasForeignKey(a => a.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(a => a.BusinessId);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.AssignedToId);
        builder.HasIndex(a => new { a.AssignedToId, a.Status });
        builder.HasIndex(a => a.DueDate);
        builder.HasIndex(a => a.IncidentId);
    }
}

