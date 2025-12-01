using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Quality;

namespace Toss.Infrastructure.Data.Configurations.Quality;

public class ChecklistItemConfiguration : IEntityTypeConfiguration<ChecklistItem>
{
    public void Configure(EntityTypeBuilder<ChecklistItem> builder)
    {
        builder.Property(i => i.Title)
            .HasMaxLength(300)
            .IsRequired();

        // Relationships
        builder.HasOne(i => i.Business)
            .WithMany()
            .HasForeignKey(i => i.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.QualityChecklist)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.QualityChecklistId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(i => i.BusinessId);
        builder.HasIndex(i => i.QualityChecklistId);
        builder.HasIndex(i => new { i.QualityChecklistId, i.Order });
    }
}

