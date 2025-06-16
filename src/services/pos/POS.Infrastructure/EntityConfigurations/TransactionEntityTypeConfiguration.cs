using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.TransactionAggregate;

namespace TossErp.POS.Infrastructure.EntityConfigurations;

public class TransactionEntityTypeConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions", POSContext.DEFAULT_SCHEMA);

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Reference)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.StoreId)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Status)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(t => t.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(t => t.PaymentMethod)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.Metadata)
            .HasMaxLength(1000);

        builder.HasIndex(t => t.Reference)
            .IsUnique();

        builder.HasIndex(t => t.StoreId);

        builder.HasIndex(t => t.Status);
    }
} 
