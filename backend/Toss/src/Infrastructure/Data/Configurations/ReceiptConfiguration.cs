using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Sales;

namespace Toss.Infrastructure.Data.Configurations;

public class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
{
    public void Configure(EntityTypeBuilder<Receipt> builder)
    {
        builder.Property(r => r.ReceiptNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.CustomerName)
            .HasMaxLength(200);

        builder.Property(r => r.CustomerPhone)
            .HasMaxLength(20);

        builder.HasIndex(r => r.ReceiptNumber)
            .IsUnique();

        builder.HasIndex(r => r.IssuedDate);
    }
}

