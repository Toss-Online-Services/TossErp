using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Projects.Domain.Entities;

namespace TossErp.Projects.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for TaskComment entity
/// </summary>
public class TaskCommentConfiguration : IEntityTypeConfiguration<TaskComment>
{
    public void Configure(EntityTypeBuilder<TaskComment> builder)
    {
        // Table configuration
        builder.ToTable("TaskComments", "projects");
        
        // Primary key
        builder.HasKey(tc => tc.Id);
        
        // Properties
        builder.Property(tc => tc.Id)
            .IsRequired();

        builder.Property(tc => tc.TaskId)
            .IsRequired();

        builder.Property(tc => tc.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(tc => tc.AuthorId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(tc => tc.AuthorName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(tc => tc.CreatedAt)
            .IsRequired();

        builder.Property(tc => tc.ModifiedAt);

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(tc => tc.TaskId)
            .HasDatabaseName("IX_TaskComments_TaskId")
            .HasFillFactor(90);

        builder.HasIndex(tc => new { tc.TaskId, tc.CreatedAt })
            .HasDatabaseName("IX_TaskComments_TaskId_CreatedAt")
            .HasFillFactor(90);

        builder.HasIndex(tc => tc.AuthorId)
            .HasDatabaseName("IX_TaskComments_AuthorId")
            .HasFillFactor(90);

        // Ignore navigation properties
        builder.Ignore(tc => tc.DomainEvents);
    }
}
