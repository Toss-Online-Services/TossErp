using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Logistics;

namespace Toss.Infrastructure.Data.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.Property(d => d.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.Email)
            .HasMaxLength(256);

        builder.Property(d => d.LicenseNumber)
            .HasMaxLength(50);

        builder.Property(d => d.VehicleType)
            .HasMaxLength(100);

        builder.Property(d => d.VehicleRegistration)
            .HasMaxLength(50);

        builder.ComplexProperty(d => d.Phone, phoneBuilder =>
        {
            phoneBuilder.Property(p => p.Number).HasMaxLength(20).IsRequired();
        });

        builder.HasOne(d => d.Business)
            .WithMany()
            .HasForeignKey(d => d.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(d => new { d.BusinessId, d.Email });
        builder.HasIndex(d => new { d.BusinessId, d.IsActive, d.IsAvailable });
    }
}

