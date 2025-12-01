using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Notifications;

namespace Toss.Infrastructure.Data.Configurations.Notifications;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.Property(c => c.LinkedType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Body)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(c => c.CreatedBy)
            .HasMaxLength(450)
            .IsRequired();

        // Relationships
        builder.HasOne(c => c.Business)
            .WithMany()
            .HasForeignKey(c => c.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.ParentComment)
            .WithMany(p => p.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(c => c.BusinessId);
        builder.HasIndex(c => new { c.BusinessId, c.LinkedType, c.LinkedId });
        builder.HasIndex(c => c.ParentCommentId);
        builder.HasIndex(c => c.Created);
    }
}

