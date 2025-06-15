using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.SyncAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class SyncLogEntityTypeConfiguration : IEntityTypeConfiguration<SyncLog>
{
    public void Configure(EntityTypeBuilder<SyncLog> builder)
    {
        builder.ToTable("SyncLogs", POSContext.DEFAULT_SCHEMA);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();

        builder.Property(x => x.StoreId).IsRequired();
        builder.Property(x => x.EntityType).IsRequired().HasMaxLength(100);
        builder.Property(x => x.EntityId).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Action).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ErrorMessage).HasMaxLength(500);
        builder.Property(x => x.RetryCount).IsRequired();
        builder.Property(x => x.LastRetryAt);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);

        builder.HasIndex(x => x.StoreId);
        builder.HasIndex(x => x.EntityType);
        builder.HasIndex(x => x.EntityId);
        builder.HasIndex(x => x.Action);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAt);

        builder.HasOne(x => x.Store)
            .WithMany()
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 
