using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Quality;

namespace Toss.Infrastructure.Data.Configurations.Quality;

public class QualityChecklistConfiguration : IEntityTypeConfiguration<QualityChecklist>
{
    public void Configure(EntityTypeBuilder<QualityChecklist> builder)
    {
        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(c => c.Business)
            .WithMany()
            .HasForeignKey(c => c.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Items)
            .WithOne(i => i.QualityChecklist)
            .HasForeignKey(i => i.QualityChecklistId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Runs)
            .WithOne(r => r.QualityChecklist)
            .HasForeignKey(r => r.QualityChecklistId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(c => c.BusinessId);
        builder.HasIndex(c => new { c.BusinessId, c.Name })
            .IsUnique();
        builder.HasIndex(c => c.IsActive);
    }
}

