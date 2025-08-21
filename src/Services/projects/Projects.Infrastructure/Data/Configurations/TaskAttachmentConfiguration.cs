using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Projects.Domain.Entities;

namespace TossErp.Projects.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for TaskAttachment entity
/// </summary>
public class TaskAttachmentConfiguration : IEntityTypeConfiguration<TaskAttachment>
{
    public void Configure(EntityTypeBuilder<TaskAttachment> builder)
    {
        // Table configuration
        builder.ToTable("TaskAttachments", "projects");
        
        // Primary key
        builder.HasKey(ta => ta.Id);
        
        // Properties
        builder.Property(ta => ta.Id)
            .IsRequired();

        builder.Property(ta => ta.TaskId)
            .IsRequired();

        builder.Property(ta => ta.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ta => ta.Description)
            .HasMaxLength(500);

        builder.Property(ta => ta.FilePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(ta => ta.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ta => ta.FileSize)
            .IsRequired();

        builder.Property(ta => ta.UploadedBy)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ta => ta.UploadedAt)
            .IsRequired();

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(ta => ta.TaskId)
            .HasDatabaseName("IX_TaskAttachments_TaskId")
            .HasFillFactor(90);

        builder.HasIndex(ta => new { ta.TaskId, ta.UploadedAt })
            .HasDatabaseName("IX_TaskAttachments_TaskId_UploadedAt")
            .HasFillFactor(90);

        builder.HasIndex(ta => ta.UploadedBy)
            .HasDatabaseName("IX_TaskAttachments_UploadedBy")
            .HasFillFactor(90);

        // Ignore navigation properties
        builder.Ignore(ta => ta.DomainEvents);
    }
}
