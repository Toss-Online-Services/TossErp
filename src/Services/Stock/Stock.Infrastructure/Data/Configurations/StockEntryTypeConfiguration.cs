using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;

namespace TossErp.Stock.Infrastructure.Data.Configurations;

public class StockEntryTypeConfiguration : IEntityTypeConfiguration<StockEntryType>
{
    public void Configure(EntityTypeBuilder<StockEntryType> builder)
    {
        builder.ToTable("StockEntryTypes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Purpose)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.AddToTransit)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.AllowNegativeStock)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.IsDisabled)
            .IsRequired()
            .HasDefaultValue(false);

        // Create unique index on Name
        builder.HasIndex(x => x.Name)
            .IsUnique();

        // Create index on Purpose for filtering
        builder.HasIndex(x => x.Purpose);

        // Create index on IsDisabled for filtering
        builder.HasIndex(x => x.IsDisabled);

        // Configure the relationship with StockEntryAggregate
        builder.HasMany(x => x.StockEntries)
            .WithOne()
            .HasForeignKey("StockEntryTypeId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
