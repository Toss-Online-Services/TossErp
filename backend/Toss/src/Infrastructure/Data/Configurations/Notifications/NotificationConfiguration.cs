using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Notifications;
using Toss.Domain.Enums;

namespace Toss.Infrastructure.Data.Configurations.Notifications;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.Property(n => n.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(n => n.Message)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(n => n.LinkedType)
            .HasMaxLength(50);

        builder.Property(n => n.ActionUrl)
            .HasMaxLength(500);

        builder.Property(n => n.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(n => n.Type)
            .HasConversion<int>();

        builder.Property(n => n.Status)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(n => n.Business)
            .WithMany()
            .HasForeignKey(n => n.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(n => n.BusinessId);
        builder.HasIndex(n => new { n.BusinessId, n.UserId, n.Status });
        builder.HasIndex(n => new { n.BusinessId, n.LinkedType, n.LinkedId });
        builder.HasIndex(n => n.Created);
    }
}

