using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.HR;

namespace Toss.Infrastructure.Data.Configurations.HR;

public class PayrollRunConfiguration : IEntityTypeConfiguration<PayrollRun>
{
    public void Configure(EntityTypeBuilder<PayrollRun> builder)
    {
        builder.Property(p => p.Gross)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.Deductions)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.Net)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.Notes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(p => p.Business)
            .WithMany()
            .HasForeignKey(p => p.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Employee)
            .WithMany(e => e.PayrollRuns)
            .HasForeignKey(p => p.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.BusinessId);
        builder.HasIndex(p => p.EmployeeId);
        builder.HasIndex(p => p.PeriodStart);
        builder.HasIndex(p => p.PeriodEnd);
        builder.HasIndex(p => new { p.EmployeeId, p.PeriodStart, p.PeriodEnd })
            .IsUnique(); // Prevent duplicate payroll runs for same employee and period
    }
}

