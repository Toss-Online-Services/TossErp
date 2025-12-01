using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.HR;
using Toss.Domain.Enums;

namespace Toss.Infrastructure.Data.Configurations.HR;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Email)
            .HasMaxLength(255);

        builder.Property(e => e.Phone)
            .HasMaxLength(50);

        builder.Property(e => e.IdNumber)
            .HasMaxLength(50);

        builder.Property(e => e.Rate)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(e => e.RateType)
            .HasConversion<int>();

        builder.Property(e => e.Notes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(e => e.Business)
            .WithMany()
            .HasForeignKey(e => e.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.AttendanceRecords)
            .WithOne(a => a.Employee)
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.PayrollRuns)
            .WithOne(p => p.Employee)
            .HasForeignKey(p => p.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.BusinessId);
        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => new { e.BusinessId, e.Name });
    }
}

