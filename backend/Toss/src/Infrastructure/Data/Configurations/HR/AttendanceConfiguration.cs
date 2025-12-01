using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.HR;

namespace Toss.Infrastructure.Data.Configurations.HR;

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.Property(a => a.Notes)
            .HasMaxLength(1000);

        builder.Property(a => a.DaysWorked)
            .HasPrecision(5, 2);

        // Relationships
        builder.HasOne(a => a.Business)
            .WithMany()
            .HasForeignKey(a => a.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Employee)
            .WithMany(e => e.AttendanceRecords)
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(a => a.BusinessId);
        builder.HasIndex(a => a.EmployeeId);
        builder.HasIndex(a => a.AttendanceDate);
        builder.HasIndex(a => new { a.EmployeeId, a.AttendanceDate })
            .IsUnique(); // Prevent duplicate attendance entries per employee per date
    }
}

