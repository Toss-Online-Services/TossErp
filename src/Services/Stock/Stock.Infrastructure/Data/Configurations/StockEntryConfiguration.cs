using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Infrastructure.Data.Configurations;

public class StockEntryConfiguration : IEntityTypeConfiguration<StockEntryAggregate>
{
    public void Configure(EntityTypeBuilder<StockEntryAggregate> builder)
    {
        builder.ToTable("StockEntries");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        // Properties
        builder.Property(x => x.EntryNumber)
            .HasConversion(
                v => v.Value,
                v => new StockEntryNo(v))
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(x => x.EntryDate).IsRequired();
        builder.Property(x => x.Reference).HasMaxLength(100);
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.Property(x => x.IsPosted).IsRequired();
        builder.Property(x => x.PostedDate);
        builder.Property(x => x.PostedBy).HasMaxLength(100);

        // Indexes
        builder.HasIndex(x => x.EntryNumber).IsUnique();
        builder.HasIndex(x => x.EntryDate);
        builder.HasIndex(x => x.Reference);
        builder.HasIndex(x => x.IsPosted);
        builder.HasIndex(x => x.PostedDate);

        // Relationships
        builder.HasMany(x => x.Details)
            .WithOne()
            .HasForeignKey(x => x.StockEntryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 
