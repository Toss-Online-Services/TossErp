using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.ArtificialIntelligence;

namespace Toss.Infrastructure.Data.Configurations;

public class AISettingsConfiguration : IEntityTypeConfiguration<AISettings>
{
    public void Configure(EntityTypeBuilder<AISettings> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ApiKey)
            .HasMaxLength(500);

        builder.Property(x => x.ApiEndpoint)
            .HasMaxLength(500);

        builder.Property(x => x.SupportedLanguages)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
                (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));

        builder.HasOne(x => x.Shop)
            .WithMany()
            .HasForeignKey(x => x.ShopId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

