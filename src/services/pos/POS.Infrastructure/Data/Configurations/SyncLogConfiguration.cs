using TossErp.POS.Domain.AggregatesModel.SyncLogAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TossErp.POS.Infrastructure.Data.Configurations;

public class SyncLogConfiguration : IEntityTypeConfiguration<SyncLog>
{
    public void Configure(EntityTypeBuilder<SyncLog> builder)
    {
        builder.ToTable("SyncLogs");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SaleId)
            .IsRequired();

        builder.Property(s => s.StoreId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.SyncedAt)
            .IsRequired();

        builder.Property(s => s.Success)
            .IsRequired();

        builder.Property(s => s.ErrorMessage)
            .HasMaxLength(500);

        builder.HasIndex(s => s.SaleId);
        builder.HasIndex(s => s.StoreId);
        builder.HasIndex(s => s.SyncedAt);
    }
} 
