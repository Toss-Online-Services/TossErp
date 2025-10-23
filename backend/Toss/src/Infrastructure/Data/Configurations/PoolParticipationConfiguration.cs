using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.GroupBuying;

namespace Toss.Infrastructure.Data.Configurations;

public class PoolParticipationConfiguration : IEntityTypeConfiguration<PoolParticipation>
{
    public void Configure(EntityTypeBuilder<PoolParticipation> builder)
    {
        builder.Property(p => p.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(p => p.Subtotal)
            .HasPrecision(18, 2);

        builder.Property(p => p.ShippingShare)
            .HasPrecision(18, 2);

        builder.Property(p => p.Total)
            .HasPrecision(18, 2);

        builder.Property(p => p.Notes)
            .HasMaxLength(1000);

        builder.HasOne(p => p.Shop)
            .WithMany()
            .HasForeignKey(p => p.ShopId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.GroupBuyPoolId, p.ShopId })
            .IsUnique();

        builder.HasIndex(p => p.JoinedDate);
    }
}

