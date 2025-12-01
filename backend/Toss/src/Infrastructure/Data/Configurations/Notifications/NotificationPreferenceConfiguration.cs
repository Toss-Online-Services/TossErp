using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Notifications;
using Toss.Domain.Enums;

namespace Toss.Infrastructure.Data.Configurations.Notifications;

public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.Property(p => p.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(p => p.NotificationType)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(p => p.Business)
            .WithMany()
            .HasForeignKey(p => p.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint: one preference per user per notification type per business
        builder.HasIndex(p => new { p.BusinessId, p.UserId, p.NotificationType })
            .IsUnique();
    }
}

