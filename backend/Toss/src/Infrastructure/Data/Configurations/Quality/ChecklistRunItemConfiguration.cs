using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Quality;

namespace Toss.Infrastructure.Data.Configurations.Quality;

public class ChecklistRunItemConfiguration : IEntityTypeConfiguration<ChecklistRunItem>
{
    public void Configure(EntityTypeBuilder<ChecklistRunItem> builder)
    {
        builder.Property(ri => ri.Notes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(ri => ri.Business)
            .WithMany()
            .HasForeignKey(ri => ri.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ri => ri.ChecklistRun)
            .WithMany(r => r.Results)
            .HasForeignKey(ri => ri.ChecklistRunId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ri => ri.ChecklistItem)
            .WithMany()
            .HasForeignKey(ri => ri.ChecklistItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(ri => ri.BusinessId);
        builder.HasIndex(ri => ri.ChecklistRunId);
        builder.HasIndex(ri => ri.ChecklistItemId);
    }
}

