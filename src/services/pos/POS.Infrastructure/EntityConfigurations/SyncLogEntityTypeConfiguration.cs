using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.SyncAggregate;

namespace TossErp.POS.Infrastructure.EntityConfigurations;

public class SyncLogEntityTypeConfiguration : IEntityTypeConfiguration<SyncLog>
{
    public void Configure(EntityTypeBuilder<SyncLog> builder)
    {
        builder.ToTable("sync_logs", "POS");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasConversion<string>();

        builder.Property(s => s.StoreId).HasConversion<string>().IsRequired();

        builder.Property(s => s.Status).HasMaxLength(20).IsRequired();
        builder.Property(s => s.Type).HasMaxLength(50).IsRequired();
        builder.Property(s => s.Message).HasMaxLength(500);
        builder.Property(s => s.Details).HasMaxLength(2000);

        builder.Property(s => s.SyncDate).IsRequired();
        builder.Property(s => s.Success).IsRequired();

        builder.HasIndex(s => s.StoreId);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.Type);
        builder.HasIndex(s => s.SyncDate);
        builder.HasIndex(s => s.Success);

        builder.HasOne(s => s.Store)
            .WithMany()
            .HasForeignKey(s => s.StoreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 
