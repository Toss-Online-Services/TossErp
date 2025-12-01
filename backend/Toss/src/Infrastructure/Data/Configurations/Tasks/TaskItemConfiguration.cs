using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Tasks;

namespace Toss.Infrastructure.Data.Configurations.Tasks;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.LinkedType)
            .HasMaxLength(50);

        builder.Property(t => t.Tags)
            .HasMaxLength(500);

        builder.Property(t => t.Status)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(t => t.Business)
            .WithMany()
            .HasForeignKey(t => t.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        // AssigneeId is a string reference to ApplicationUser - no navigation property in Domain layer

        // Indexes
        builder.HasIndex(t => t.BusinessId);
        builder.HasIndex(t => new { t.BusinessId, t.Status });
        builder.HasIndex(t => new { t.BusinessId, t.LinkedType, t.LinkedId });
        builder.HasIndex(t => t.AssigneeId);
        builder.HasIndex(t => t.DueDate);
    }
}

