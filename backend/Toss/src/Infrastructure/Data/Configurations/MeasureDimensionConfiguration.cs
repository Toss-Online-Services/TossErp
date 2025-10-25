using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Directory;

namespace Toss.Infrastructure.Data.Configurations;

public class MeasureDimensionConfiguration : IEntityTypeConfiguration<MeasureDimension>
{
    public void Configure(EntityTypeBuilder<MeasureDimension> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.SystemKeyword)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Ratio)
            .HasPrecision(18, 8);
    }
}

